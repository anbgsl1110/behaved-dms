using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Threading;
using Dapper;
using System.Collections;

namespace Dapper.Contrib.Extensions
{
    public class SqlEscape
    {
        public string LeftEscape;
        public string RightEscape;
    }

    public static class SqlMapperExtensions
    {
        public interface IProxy
        {
            bool IsDirty { get; set; }
        }

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> InsertQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> UpdateQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>() {
            		                                                                        {"mysqlconnection", new MySqlServerAdapter()} //,
                                                                                            //{"sqlconnection", new SqlServerAdapter()},
                                                                                            //{"npgsqlconnection", new PostgresAdapter()},
                                                                                            //{"sqliteconnection", new SQLiteAdapter()}
																						};
        private static readonly Dictionary<string, SqlEscape> EscapeDictionary = new Dictionary<string, SqlEscape>() {
            {"mysqlconnection", new SqlEscape() {LeftEscape="`", RightEscape="`"}}
        };

        private static IEnumerable<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pi;
            if (ComputedProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi;
            }

            var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ComputedAttribute)).ToList();

            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

        private static IEnumerable<PropertyInfo> KeyPropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pi;
            if (KeyProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi;
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count == 0)
            {
                var idProp = allProperties.Where(p => p.Name.ToLower() == "id" || p.Name.ToLower() == "orgid").FirstOrDefault();
                if (idProp != null)
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis;
            }

            var properties = type.GetProperties().Where(IsWriteable).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        public static string InsertQueriesCache(Type type, SqlEscape escape = null)
        {
            string sql;
            if (!InsertQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var name = GetTableName(type);

                var sbColumnList = new StringBuilder(null);
                var allProperties = TypePropertiesCache(type);
                var keyProperties = KeyPropertiesCache(type);
                var computedProperties = ComputedPropertiesCache(type);
                var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties));

                for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count(); i++)
                {
                    var property = allPropertiesExceptKeyAndComputed.ElementAt(i);
                    var keyName = escape == null ? property.Name : string.Format("{0}{1}{2}", escape.LeftEscape, property.Name, escape.RightEscape);
                    sbColumnList.AppendFormat("{0}", keyName);
                    if (i < allPropertiesExceptKeyAndComputed.Count() - 1)
                        sbColumnList.Append(", ");
                }

                var sbParameterList = new StringBuilder(null);
                for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count(); i++)
                {
                    var property = allPropertiesExceptKeyAndComputed.ElementAt(i);
                    sbParameterList.AppendFormat("@{0}", property.Name);
                    if (i < allPropertiesExceptKeyAndComputed.Count() - 1)
                        sbParameterList.Append(", ");
                }
                sql = String.Format("insert into {0} ({1}) values ({2})", name, sbColumnList.ToString(), sbParameterList.ToString());
                InsertQueries[type.TypeHandle] = sql;
            }
            return sql;
        }

        private static string UpdateQueriesCache(Type type, SqlEscape escape = null)
        {
            string sql;
            if (!UpdateQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var keyProperties = KeyPropertiesCache(type);
                if (!keyProperties.Any())
                    throw new ArgumentException("Entity must have at least one [Key] property");

                var name = GetTableName(type);

                var sb = new StringBuilder();
                sb.AppendFormat("update {0} set ", name);

                var allProperties = TypePropertiesCache(type);
                var computedProperties = ComputedPropertiesCache(type);
                var nonIdProps = allProperties.Except(keyProperties.Union(computedProperties));

                for (var i = 0; i < nonIdProps.Count(); i++)
                {
                    var property = nonIdProps.ElementAt(i);
                    var keyName = escape == null ? property.Name : string.Format("{0}{1}{2}", escape.LeftEscape, property.Name, escape.RightEscape);
                    sb.AppendFormat("{0} = @{1}", keyName, property.Name);
                    if (i < nonIdProps.Count() - 1)
                        sb.AppendFormat(", ");
                }
                sb.Append(" where ");
                for (var i = 0; i < keyProperties.Count(); i++)
                {
                    var property = keyProperties.ElementAt(i);
                    var keyName = escape == null ? property.Name : string.Format("{0}{1}{2}", escape.LeftEscape, property.Name, escape.RightEscape);
                    sb.AppendFormat("{0} = @{1}", keyName, property.Name);
                    if (i < keyProperties.Count() - 1)
                        sb.AppendFormat(" and ");
                }
                sql = sb.ToString();
                UpdateQueries[type.TypeHandle] = sql;
            }
            return sql;
        }

        public static string GetUpdatePartQuery(Type type, object entity, SqlEscape escape = null)
        {
            var keyProperties = KeyPropertiesCache(type);
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");
            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);
            var keyNames = keyProperties.Select(p => p.Name.ToLower());

            IEnumerable<string> nonIdPropNames;
            if (entity is IEnumerable<KeyValuePair<string, object>>)
            {
                var obj = entity as IEnumerable<KeyValuePair<string, object>>;
                nonIdPropNames = obj.Where(p => !keyNames.Contains(p.Key.ToLower())).Select(p => p.Key);
            }
            else
            {
                var allProperties = entity.GetType().GetProperties().Where(IsWriteable).ToArray();
                nonIdPropNames = allProperties.Where(p => !keyNames.Contains(p.Name.ToLower())).Select(p => p.Name);
            }

            for (var i = 0; i < nonIdPropNames.Count(); i++)
            {
                var property = nonIdPropNames.ElementAt(i);
                var keyName = escape == null ? property : string.Format("{0}{1}{2}", escape.LeftEscape, property, escape.RightEscape);
                sb.AppendFormat("{0} = @{1}", keyName, property);
                if (i < nonIdPropNames.Count() - 1)
                    sb.AppendFormat(", ");
            }
            sb.Append(" where ");
            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                var keyName = escape == null ? property.Name : string.Format("{0}{1}{2}", escape.LeftEscape, property.Name, escape.RightEscape);
                sb.AppendFormat("{0} = @{1}", keyName, property.Name);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            return sb.ToString();
        }

        public static string GetUpdatePartQuery(Type type, IDictionary<string, object> source, IDictionary<string, object> condition, SqlEscape escape = null)
        {
            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);
            var keyNames = condition.Select(p => p.Key).ToList();

            IEnumerable<string> nonIdPropNames = source.Select(p => p.Key);

            for (var i = 0; i < nonIdPropNames.Count(); i++)
            {
                var property = nonIdPropNames.ElementAt(i);
                var keyName = escape == null ? property : string.Format("{0}{1}{2}", escape.LeftEscape, property, escape.RightEscape);
                sb.AppendFormat("{0} = @{1}", keyName, property);
                if (i < nonIdPropNames.Count() - 1)
                    sb.AppendFormat(", ");
            }
            sb.Append(" where ");
            for (var i = 0; i < keyNames.Count(); i++)
            {
                var property = keyNames.ElementAt(i);
                var keyName = escape == null ? property : string.Format("{0}{1}{2}", escape.LeftEscape, property, escape.RightEscape);

                if (source.ContainsKey(property))
                {
                    string rename = property + DateTime.Now.Ticks;
                    condition.Add(rename, condition[property]);
                    condition.Remove(property);
                    property = rename;
                }

                sb.AppendFormat("{0} = @{1}", keyName, property);
                if (i < keyNames.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            return sb.ToString();
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false);
            if (attributes.Length == 1)
            {
                WriteAttribute write = (WriteAttribute)attributes[0];
                return write.Write;
            }
            return true;
        }

        private static string GetTableName(Type type)
        {
            string name;
            if (!TypeTableName.TryGetValue(type.TypeHandle, out name))
            {
                name = type.Name;
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);

                //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableattr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as
                    dynamic;
                if (tableattr != null)
                    name = tableattr.Name;
                TypeTableName[type.TypeHandle] = name;
            }
            return name;
        }

        /// <summary>
        /// Returns a single entity by a single id from table "Ts". T must be of interface type. 
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension. 
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <returns>Entity of T</returns>
        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            string sql;
            if (!GetQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var keys = KeyPropertiesCache(type);
                if (keys.Count() > 1)
                    throw new DataException("Get<T> only supports an entity with a single [Key] property");
                if (keys.Count() == 0)
                    throw new DataException("Get<T> only supports en entity with a [Key] property");

                var onlyKey = keys.First();

                var name = GetTableName(type);

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface      
                SqlEscape escape = GetEscape(connection);
                var keyName = escape == null ? onlyKey.Name : string.Format("{0}{1}{2}", escape.LeftEscape, onlyKey.Name, escape.RightEscape);
                sql = "select * from " + name + " where " + keyName + " = @id";
                GetQueries[type.TypeHandle] = sql;
            }

            var dynParms = new DynamicParameters();
            dynParms.Add("@id", id);

            T obj = null;

            if (type.IsInterface)
            {
                var res = connection.Query(sql, dynParms).FirstOrDefault() as IDictionary<string, object>;

                if (res == null)
                    return (T)((object)null);

                obj = ProxyGenerator.GetInterfaceProxy<T>();

                foreach (var property in TypePropertiesCache(type))
                {
                    var val = res[property.Name];
                    property.SetValue(obj, val, null);
                }

                ((IProxy)obj).IsDirty = false;   //reset change tracking and return
            }
            else
            {
                obj = connection.Query<T>(sql, dynParms, transaction: transaction, commandTimeout: commandTimeout).FirstOrDefault();
            }
            return obj;
        }

        /// <summary>
        /// Returns a list of entites from table "Ts".  
        /// Id of T must be marked with [Key] attribute.
        /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
        /// for optimal performance. 
        /// </summary>
        /// <typeparam name="T">Interface or type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>Entity of T</returns>
        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var cacheType = typeof(List<T>);

            string sql;
            if (!GetQueries.TryGetValue(cacheType.TypeHandle, out sql))
            {
                var keys = KeyPropertiesCache(type);
                if (keys.Count() > 1)
                    throw new DataException("Get<T> only supports an entity with a single [Key] property");
                if (!keys.Any())
                    throw new DataException("Get<T> only supports en entity with a [Key] property");

                var name = GetTableName(type);

                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                sql = "select * from " + name;
                GetQueries[cacheType.TypeHandle] = sql;
            }


            if (!type.IsInterface) return connection.Query<T>(sql, null, transaction, commandTimeout: commandTimeout);

            var result = connection.Query(sql);
            var list = new List<T>();
            foreach (IDictionary<string, object> res in result)
            {
                var obj = ProxyGenerator.GetInterfaceProxy<T>();
                foreach (var property in TypePropertiesCache(type))
                {
                    var val = res[property.Name];
                    property.SetValue(obj, val, null);
                }
                ((IProxy)obj).IsDirty = false;   //reset change tracking and return
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <returns>Identity of inserted entity</returns>
        public static long Insert<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            ISqlAdapter adapter = GetFormatter(connection);
            SqlEscape escape = GetEscape(connection);
            var cmd = InsertQueriesCache(type, escape);
            return connection.ExecuteScalar<long>(string.Format("{0};{1}", cmd, adapter.GetIdentity()), entityToInsert, transaction: transaction, commandTimeout: commandTimeout);
        }

        public static int Insert<T>(this IDbConnection connection, IEnumerable<T> entitiesToInsert, IDbTransaction transaction = null) where T : class
        {
            var type = typeof(T);
            SqlEscape escape = GetEscape(connection);
            var cmd = InsertQueriesCache(type, escape);
            return connection.Execute(cmd, entitiesToInsert, transaction);
        }

        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var proxy = entityToUpdate as IProxy;
            if (proxy != null)
            {
                if (!proxy.IsDirty) return false;
            }
            var type = typeof(T);
            SqlEscape escape = GetEscape(connection);
            var updated = connection.Execute(UpdateQueriesCache(type, escape), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        public static bool UpdatePart<T>(this IDbConnection connection, object entityToUpdate, IDbTransaction transaction = null) where T : class
        {
            var type = typeof(T);
            SqlEscape escape = GetEscape(connection);
            var cmd = GetUpdatePartQuery(type, entityToUpdate, escape);
            return connection.Execute(cmd, entityToUpdate, transaction) > 0;
        }

        public static int UpdatePart<T>(this IDbConnection connection, IDictionary<string, object> source, IDictionary<string, object> condition, IDbTransaction transaction = null) where T : class
        {
            var type = typeof(T);
            SqlEscape escape = GetEscape(connection);
            var cmd = GetUpdatePartQuery(type, source, condition, escape);
            return connection.Execute(cmd, source.Union(condition), transaction);
        }

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
                throw new ArgumentException("Cannot Delete null Object", "entityToDelete");

            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (keyProperties.Count() == 0)
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);
            SqlEscape escape = GetEscape(connection);
            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                var keyName = escape == null ? property.Name : string.Format("{0}{1}{2}", escape.LeftEscape, property.Name, escape.RightEscape);
                sb.AppendFormat("{0} = @{1}", keyName, property.Name);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction: transaction, commandTimeout: commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// Delete all entities in the table related to the type T.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>true if deleted, false if none found</returns>
        public static bool DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var type = typeof(T);
            var name = GetTableName(type);
            var statement = String.Format("delete from {0}", name);
            var deleted = connection.Execute(statement, null, transaction: transaction, commandTimeout: commandTimeout);
            return deleted > 0;
        }

        public static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            string name = connection.GetType().Name.ToLower();
            if (!AdapterDictionary.ContainsKey(name))
                return new MySqlServerAdapter();
            return AdapterDictionary[name];
        }

        public static SqlEscape GetEscape(IDbConnection connection)
        {
            string name = connection.GetType().Name.ToLower();
            if (!EscapeDictionary.ContainsKey(name))
                return null;
            return EscapeDictionary[name];
        }

        class ProxyGenerator
        {
            private static readonly Dictionary<Type, object> TypeCache = new Dictionary<Type, object>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
                var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName { Name = name },
                    AssemblyBuilderAccess.Run);       //NOTE: to save, use RunAndSave

                return assemblyBuilder;
            }

            public static T GetClassProxy<T>()
            {
                // A class proxy could be implemented if all properties are virtual
                //  otherwise there is a pretty dangerous case where internal actions will not update dirty tracking
                throw new NotImplementedException();
            }


            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);

                object k;
                if (TypeCache.TryGetValue(typeOfT, out k))
                {
                    return (T)k;
                }
                var assemblyBuilder = GetAsmBuilder(typeOfT.Name);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

                var interfaceType = typeof(Dapper.Contrib.Extensions.SqlMapperExtensions.IProxy);
                var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);

                //create our _isDirty field, which implements IProxy
                var setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (var property in typeof(T).GetProperties())
                {
                    var isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

                var generatedType = typeBuilder.CreateType();

                //assemblyBuilder.Save(name + ".dll");  //NOTE: to save, uncomment

                var generatedObject = Activator.CreateInstance(generatedType);

                TypeCache.Add(typeOfT, generatedObject);
                return (T)generatedObject;
            }


            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                var propType = typeof(bool);
                var field = typeBuilder.DefineField("_" + "IsDirty", propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty("IsDirty",
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                                                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + "IsDirty",
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);
                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + "IsDirty",
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(Dapper.Contrib.Extensions.SqlMapperExtensions.IProxy).GetMethod("get_" + "IsDirty");
                var setMethod = typeof(Dapper.Contrib.Extensions.SqlMapperExtensions.IProxy).GetMethod("set_" + "IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                //Define the field and the property 
                var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty(propertyName,
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Virtual |
                                                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);

                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });

                //store value in private field and set the isdirty flag
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldc_I4_1);
                currSetIL.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIL.Emit(OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity)
                {
                    var keyAttribute = typeof(KeyAttribute);
                    var myConstructorInfo = keyAttribute.GetConstructor(new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[] { });
                    property.SetCustomAttribute(attributeBuilder);
                }

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(T).GetMethod("get_" + propertyName);
                var setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }

        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }
        public string Name { get; private set; }
    }

    // do not want to depend on data annotations that is not in client profile
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class WriteAttribute : Attribute
    {
        public WriteAttribute(bool write)
        {
            Write = write;
        }
        public bool Write { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute
    {
    }
}

public interface ISqlAdapter
{
    string GetIdentity();
}

public class MySqlServerAdapter : ISqlAdapter
{
    public string GetIdentity()
    {
        return "select LAST_INSERT_ID() Id";
        //int id = (int)r.First().id;
        //if (keyProperties.Any())
        //    keyProperties.First().SetValue(entityToInsert, id, null);
    }
}