using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using DbDocGenerate.Common;
using DbDocGenerate.Plugin;

namespace DbDocGenerate.Data
{
    /// <summary>
    /// 
    /// by xie 2015.9.25
    /// </summary>
    public class Paging
    {
        private bool m_IsCache = false;  // 是否缓存
        private int m_CacheTime = 60;    // 缓存时长
        private string m_CachePrefix = "sql:";    // Key的前缀
        private PagerInfo m_Info;
        private string m_OrderBy = null;
        private bool m_HasRecordCount = true;
        private int m_RecordCount = -1;


        public int RecordCount
        {
            get { return m_RecordCount; }
        }


        /// <summary>
        /// 字段列表 x.ID,y.Name,c.Title
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// 表名 table1 A inner join table2 B on A.ID=B.LinkID
        /// </summary>
        public string Tables { get; set; }
        /// <summary>
        /// where 1=1
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// group by y.Name,c.Title having count(*) > 1
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// e.g: order by id desc
        /// </summary>
        public string OrderBy
        {
            get
            {
                if (!string.IsNullOrEmpty(OrderField))
                {
                    return "Order by " + OrderField + " " + m_Info.OrderType;
                }
                else
                {
                    return m_OrderBy;
                }
            }
            set
            {
                m_OrderBy = value;
            }
        }

        /// <summary>
        /// e.g: addtime
        /// </summary>
        private string m_OrderField = string.Empty;
        private string OrderField
        {
            get
            {
                if (string.IsNullOrEmpty(m_OrderField))
                {
                    // 非法验证
                    if (!string.IsNullOrEmpty(m_Info.OrderField))
                    {
                        string tmp = m_Info.OrderField.ToLower();
                        foreach (var item in Fields.Split(','))
                        {
                            if (item.Substring(item.IndexOf('.') + 1).ToLower().Trim() == tmp)
                            {
                                m_OrderField = item;
                                break;
                            }
                            else if (item.IndexOf(" as ") != -1)
                            {
                                string[] arr = Lib.splitstr(item, " as ");
                                if (arr.Length == 2)
                                {
                                    if (arr[1].ToLower().Trim() == tmp)
                                    {
                                        m_OrderField = arr[0];
                                        break;
                                    }
                                }
                                if (!string.IsNullOrEmpty(m_OrderField)) break;
                            }
                        }
                    }
                }

                return m_OrderField;
            }
            set
            {
                m_OrderField = value;
            }
        }

        /// <summary>
        /// 是否需要记录总数
        /// </summary>
        public bool HasRecordCount { get { return m_HasRecordCount; } set { m_HasRecordCount = value; } }

        /// <summary>
        /// 是否缓存查询结果
        /// </summary>
        public bool IsCache { get { return m_IsCache; } set { m_IsCache = value; } }
        /// <summary>
        /// 缓存时长，单位秒
        /// </summary>
        public int CacheTime { get { return m_CacheTime; } set { m_CacheTime = value; } }
        /// <summary>
        /// 缓存Key的前缀
        /// </summary>
        public string CachePrefix { get { return m_CachePrefix; } set { m_CachePrefix = value; } }

        public Paging(PagerInfo info)
        {
            m_Info = info;
        }

        /// <summary>
        /// 取得分页的SQL语句
        /// </summary>
        public string GetPageSql(bool ispc = true)
        {
            if (m_Info.PageSize <= 0) { m_Info.PageSize = 20; }

            //if (RecordCount % PageSize == 0) { PageCount = RecordCount / PageSize; }
            //else { PageCount = RecordCount / PageSize + 1; }

            //if (CurrentPage > PageCount && PageCount > 0) CurrentPage = PageCount;
            //if (CurrentPage < 1) CurrentPage = 1;

            string Sql = null;

            if (m_Info.CurrentPage == 1)
            { Sql = "select top " + m_Info.PageSize + " " + Fields + " from " + Tables + " " + Where + " " + Group + " " + OrderBy; }
            else
            {
                /*
                Sql = "select top " + PageSize + " " + AllFields + " from " + Tables;
                if (!string.IsNullOrEmpty(Where))
                {
                    MatchCollection mc = Regex.Matches(" " + Where, @"\ where\ ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    Match m = mc[0];    //替换第一个
                    string _where = new Regex(m.Value).Replace(" " + Where, " where (", 1, m.Index);
                    // string _where = Regex.Replace(" " + Where, @"\ where\ ", " where (", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    Sql += _where + ") and (";
                }
                else
                {
                    Sql += " where (";
                }
                Sql += KeyField + " not in (select top " + (CurrentPage - 1) * PageSize + " " + KeyField + " from " + Tables + " " + Where;
                if (!string.IsNullOrEmpty(Group))
                {
                    // Sql += " Group by " + KeyField; 
                    Sql += " " + Group;
                }
                Sql += " " + OrderFields;
                Sql += ")) " + Group + " " + OrderFields;
                 */

                //校正请求页面索引
                int page = m_Info.CurrentPage;
                if (ispc)
                {
                    AdjustPage(m_Info.PageSize, ref page);
                }
                var start = m_Info.PageSize * (page - 1) + 1;
                var end = m_Info.PageSize * page;

                Sql = "select * from (select " + Fields + ",row_number() over(" + OrderBy + ") as row_number from " + Tables + " " + Where + " " + Group + " ) tbl where row_number between " + start + " and " + end;
            }
            return Sql;
        }


        /// <summary>
        /// 校正不合理分页请求数据，小于1请求第一页，大于最大值，请求最后一页
        /// </summary>
        /// <param name="page"></param>
        private void AdjustPage(int pageSize, ref int page)
        {
            if (m_RecordCount == 0 || pageSize == 0) return;

            if (page < 1) page = 1;

            int maxPage = (int)Math.Ceiling((double)m_RecordCount / pageSize);
            if (page > maxPage) page = maxPage;

        }



        /// <summary>
        /// 处理记录总数
        /// </summary>
        /// <param name="connection"></param>
        private void DoRecordCount(DbConnection connection = null)
        {
            if (!HasRecordCount) return;

            string sql = "";
            if (!string.IsNullOrEmpty(Group))
            { sql = "select count(*) from (select count(*) as rc from " + Tables + " " + Where + " " + Group + ") as a"; }
            else { sql = "select count(*) from " + Tables + " " + Where; }

            /*if (IsCache)    // 读取缓存
            {
                m_RecordCount = Utils.StrToInt(StudyRedisService.Instance.Get<string>(CachePrefix + sql), -2);
                if (m_RecordCount > -2)
                {
                    return;
                }
            }*/

            if (connection == null)
            {
                m_RecordCount = (int)DbHelper.ExecuteScalar(sql);
            }
            else
            {
                m_RecordCount = (int)DbHelper.ExecuteScalar(connection, CommandType.Text, sql);
            }

            /*// 写入缓存
            if (IsCache) StudyRedisService.Instance.Set<string>(CachePrefix + sql, m_RecordCount.ToString(), DateTime.Now.AddSeconds(CacheTime));*/
        }


        /// <summary>
        /// 处理记录总数
        /// </summary>
        /// <param name="dbkey"></param>
        /// <param name="connection"></param>
        private void DoRecordCount2(string dbkey, DbConnection connection = null)
        {
            if (!HasRecordCount) return;

            string sql = "";
            if (!string.IsNullOrEmpty(Group))
            { sql = "select count(*) from (select count(*) as rc from " + Tables + " " + Where + " " + Group + ") as a"; }
            else { sql = "select count(*) from " + Tables + " " + Where; }

            /*if (IsCache)    // 读取缓存
            {
                m_RecordCount = Utils.StrToInt(
                    StudyRedisService.Instance.HGet(dbkey, CachePrefix + Utils.MD5(sql).ToLower()), -2);
                if (m_RecordCount > -2)
                {
                    return;
                }
            }*/

            if (connection == null)
            {
                m_RecordCount = (int)DbHelper.ExecuteScalar(sql);
            }
            else
            {
                m_RecordCount = (int)DbHelper.ExecuteScalar(connection, CommandType.Text, sql);
            }

            /*// 写入缓存
            if (IsCache)
                StudyRedisService.Instance.HSet(dbkey, CachePrefix + Utils.MD5(sql).ToLower(), m_RecordCount.ToString());*/
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public System.Data.IDataReader GetData(bool ispc = true)
        {
            if (m_RecordCount == -1) { DoRecordCount(); }

            return DbHelper.ExecuteReader(CommandType.Text, GetPageSql(ispc));
        }

        public System.Data.IDataReader GetData(DbConnection connection, bool ispc = true)
        {
            if (connection == null) return GetData(ispc);

            if (m_RecordCount == -1) { DoRecordCount(connection); }

            return DbHelper.ExecuteReader(connection, CommandType.Text, GetPageSql(ispc));
        }

        public List<Dictionary<string, object>> GetList(bool ispc = true)
        {
            return GetList(null, ispc);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ispc">时候pc使用</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetList(DbConnection connection, bool ispc = true)
        {
            if (m_RecordCount == -1) { DoRecordCount(connection); }

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string pageSql = GetPageSql(ispc);

            /*if (IsCache)    // 读取缓存
            {
                list = StudyRedisService.Instance.Get<List<Dictionary<string, object>>>(CachePrefix + pageSql);

                if (list != null)
                {
                    return list;
                }
            }*/

            list = new List<Dictionary<string, object>>();

            using (System.Data.IDataReader r = GetData(connection, ispc))
            {
                while (r.Read())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();

                    foreach (var item in Fields.Split(','))
                    {
                        string field = item.Substring(item.IndexOf('.') + 1).Trim();

                        if (field.IndexOf(" as ") != -1)
                        {
                            string[] arrField = Lib.splitstr(field, " as ");
                            field = arrField[arrField.Length - 1].Trim();
                        }
                        if (field.IndexOf(" ") != -1)
                        {
                            string[] arrField = Lib.splitstr(field, " ");

                            field = arrField[arrField.Length - 1].Trim();
                        }

                        row.Add(field, r[field]);
                    }

                    list.Add(row);
                }
            }

            /*// 写入缓存
            if (IsCache) StudyRedisService.Instance.Set<List<Dictionary<string, object>>>(CachePrefix + pageSql, list, DateTime.Now.AddSeconds(CacheTime));
*/
            return list;
        }

        public List<T> GetList<T>(DbConnection connection)
        {
            if (m_RecordCount == -1) { DoRecordCount(connection); }

            List<T> list = new List<T>();
            string pageSql = GetPageSql();

            /*if (IsCache)    // 读取缓存
            {
                list = StudyRedisService.Instance.Get<List<T>>(CachePrefix + pageSql);

                if (list != null)
                {
                    return list;
                }
            }*/

            list = new List<T>();

            using (System.Data.IDataReader r = GetData(connection))
            {
                List<string> fileds = new List<string>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    fileds.Add(r.GetName(i).ToLower());
                }

                while (r.Read())
                {
                    T t = Activator.CreateInstance<T>();

                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        Type type = pi.PropertyType;

                        if (fileds.IndexOf(pi.Name.ToLower()) == -1) continue;

                        if (type.FullName == "System.UInt32")
                        {
                            pi.SetValue(t, Convert.ToUInt32(r[pi.Name]), null);
                        }
                        else
                        {
                            if (DBNull.Value == r[pi.Name])
                            {
                                pi.SetValue(t, null, null);
                            }
                            else
                            {
                                pi.SetValue(t, r[pi.Name], null);
                            }
                        }
                    }

                    list.Add(t);
                }
            }

            /*// 写入缓存
            if (IsCache) StudyRedisService.Instance.Set<List<T>>(CachePrefix + pageSql, list, DateTime.Now.AddSeconds(CacheTime));
*/
            return list;
        }

        public List<T> GetList<T>(DbConnection connection, bool ispc = true)
        {
            if (m_RecordCount == -1) { DoRecordCount(connection); }

            List<T> list = new List<T>();
            string pageSql = GetPageSql(ispc);

            if (IsCache)    // 读取缓存
            {
                /*list = StudyRedisService.Instance.Get<List<T>>(CachePrefix + pageSql);
*/
                if (list != null)
                {
                    return list;
                }
            }

            list = new List<T>();

            using (System.Data.IDataReader r = GetData(connection, ispc))
            {
                List<string> fileds = new List<string>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    fileds.Add(r.GetName(i).ToLower());
                }

                while (r.Read())
                {
                    T t = Activator.CreateInstance<T>();

                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        Type type = pi.PropertyType;

                        if (fileds.IndexOf(pi.Name.ToLower()) == -1) continue;

                        if (type.FullName == "System.UInt32")
                        {
                            pi.SetValue(t, Convert.ToUInt32(r[pi.Name]), null);
                        }
                        else
                        {
                            if (DBNull.Value == r[pi.Name])
                            {
                                pi.SetValue(t, null, null);
                            }
                            else
                            {
                                pi.SetValue(t, r[pi.Name], null);
                            }
                        }
                    }

                    list.Add(t);
                }
            }

            /*// 写入缓存
            if (IsCache) StudyRedisService.Instance.Set<List<T>>(CachePrefix + pageSql, list, DateTime.Now.AddSeconds(CacheTime));
*/
            return list;
        }

        public List<T> GetList2<T>(DbConnection connection, string dbkey)
        {
            if (m_RecordCount == -1) { DoRecordCount2(dbkey, connection); }
            string pageSql = GetPageSql();
            List<T> list = new List<T>();

            /*if (IsCache)    // 读取缓存
            {
                var result = StudyRedisService.Instance.HGet(dbkey, CachePrefix + Utils.MD5(pageSql).ToLower());

                if (result != null)
                {
                    var newresult = JsonHelper.DeserializeObject<List<T>>(result);
                    return newresult;
                }
            }*/

            list = new List<T>();

            using (System.Data.IDataReader r = GetData(connection))
            {
                List<string> fileds = new List<string>();
                for (int i = 0; i < r.FieldCount; i++)
                {
                    fileds.Add(r.GetName(i).ToLower());
                }

                while (r.Read())
                {
                    T t = Activator.CreateInstance<T>();

                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        Type type = pi.PropertyType;

                        if (fileds.IndexOf(pi.Name.ToLower()) == -1) continue;

                        if (type.FullName == "System.UInt32")
                        {
                            pi.SetValue(t, Convert.ToUInt32(r[pi.Name]), null);
                        }
                        else
                        {
                            if (DBNull.Value == r[pi.Name])
                            {
                                pi.SetValue(t, null, null);
                            }
                            else
                            {
                                pi.SetValue(t, r[pi.Name], null);
                            }
                        }
                    }

                    list.Add(t);
                }
            }

            var reslist = JsonHelper.SerializeObject(list);

            /*// 写入缓存
            if (IsCache) StudyRedisService.Instance.HSet(dbkey, CachePrefix + Utils.MD5(pageSql).ToLower(), reslist);*/

            return list;
        }
    }
}
