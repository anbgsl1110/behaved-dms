using Dapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace OMTB.Component.Data.Query
{
    public sealed class QueryManage
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public QueryManage(IUnitOfWork uw)
        {
            this.UnitOfWork = uw;
        }

        public IEnumerable<T> GetList<T>(string sql, string countSql, out long total)
        {
            total = this.UnitOfWork.Connection.ExecuteScalar<long>(countSql);
            return GetList<T>(sql);
        }

        public IEnumerable<T> GetList<T>(string sql)
        {
            return this.UnitOfWork.Connection.Query<T>(sql);
        }

        public int GetExecuteQuery(string sql)
        {
            return this.UnitOfWork.Connection.Execute(sql);
        }

        public int GetExecuteQuery(string sql, object entity)
        {
            return this.UnitOfWork.Connection.Execute(sql, entity);
        }

        public T GetScalar<T>(string sql)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<T>(sql);
        }

        public T GetScalar<T>(string sql, object entity)
        {
            return this.UnitOfWork.Connection.ExecuteScalar<T>(sql, entity);
        }

        public DataTable GetDataTable(string sql)
        {
            return GetDataTable(UnitOfWork.Connection.ExecuteReader(sql));
        }

        private DataTable GetDataTable(IDataReader dataReader)
        {
            using (dataReader)
            {
                //把那个DataReader转化成DataTable。
                DataTable datatable = new DataTable();
                DataTable schemaTable = dataReader.GetSchemaTable();
                //动态添加列
                try
                {
                    foreach (DataRow myRow in schemaTable.Rows)
                    {
                        DataColumn myDataColumn = new DataColumn();
                        myDataColumn.DataType = myRow[11] as Type;
                        var columnName = myRow[0].ToString();
                        myDataColumn.ColumnName = datatable.Columns.Contains(columnName) ?
                            string.Format("{0}.{1}", myRow[10].ToString().ToLower(), columnName) : columnName;
                        myDataColumn.AllowDBNull = Convert.ToBoolean(myRow[12]);
                        datatable.Columns.Add(myDataColumn);
                    }
                    //添加数据
                    while (dataReader.Read())
                    {
                        DataRow myDataRow = datatable.NewRow();
                        for (int i = 0; i < schemaTable.Rows.Count; i++)
                        {
                            //myDataRow[i] = dataReader[i];
                            Type type = dataReader[i].GetType();
                            switch (type.Name)
                            {
                                case "String":
                                    myDataRow[i] = (string)dataReader[i];
                                    break;
                                case "Int16":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (short?)dataReader[i] : (short)dataReader[i];
                                    break;
                                case "Int32":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (int?)dataReader[i] : (int)dataReader[i];
                                    break;
                                case "Int64":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (long?)dataReader[i] : (long)dataReader[i];
                                    break;
                                case "DateTime":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (DateTime?)dataReader[i] : (DateTime)dataReader[i];
                                    break;
                                case "Decimal":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (decimal?)dataReader[i] : (decimal)dataReader[i];
                                    break;
                                case "Char":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (char?)dataReader[i] : (char)dataReader[i];
                                    break;
                                case "Double":
                                    myDataRow[i] = datatable.Columns[i].AllowDBNull ? (double?)dataReader[i] : (double)dataReader[i];
                                    break;
                                default:
                                    myDataRow[i] = dataReader[i];
                                    break;
                            }
                        }
                        datatable.Rows.Add(myDataRow);
                        myDataRow = null;
                    }
                    schemaTable = null;
                    return datatable;
                }
                catch (Exception ex)
                {
                    throw new Exception("转换出错出错!", ex);
                }
            }
        }

        public IEnumerable<T> GetList<T>(string sql, object entity)
        {
            return this.UnitOfWork.Connection.Query<T>(sql, entity);
        }

        public DataTable GetDataTable(string sql, object entity)
        {
            return GetDataTable(UnitOfWork.Connection.ExecuteReader(sql, entity));
        }
    }
}
