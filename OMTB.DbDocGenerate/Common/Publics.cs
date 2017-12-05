using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using DbDocGenerate.Data;
using DbDocGenerate.Plugin;
using MySql.Data.MySqlClient;

namespace DbDocGenerate.Common
{
    public class Publics
    {
        public static T CreateEntity<T>(System.Data.IDataReader dr)
        {
            T t = Activator.CreateInstance<T>();

            using (System.Data.IDataReader r = dr)
            {
                if (r.Read())
                {
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        if (!pi.CanRead || !pi.CanWrite) continue;

                        Type type = pi.PropertyType;


                        if (!ReaderExists(dr, pi.Name))
                        {
                            pi.SetValue(t, DefaultForType(type), null);
                            continue;
                        }



                        if (type.FullName == "System.UInt32")
                        {
                            pi.SetValue(t, Convert.ToUInt32(r[pi.Name]), null);
                        }
                        else if (type.FullName == "System.Guid")
                        {
                            pi.SetValue(t, new Guid(r[pi.Name].ToString()), null);
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
                }
            }
            return t;
        }

        public static object DefaultForType(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        public static bool ReaderExists(IDataReader dr, string columnName)
        {
            int count = dr.FieldCount;
            for (int i = 0; i < count; i++)
            {
                if (dr.GetName(i).Equals(columnName))
                {
                    return true;
                }
            }
            return false;
        }

        public static T CreateEntity<T>(string sql)
        {
            return CreateEntity<T>(DbHelper.ExecuteReader(sql));
        }

        /// <summary>
        /// added by lt 2016.4.7 增加另一库读法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T CreateEntity<T>(SqlConnection conn, string sql)
        {
            return CreateEntity<T>(DbHelper.ExecuteReader(conn, CommandType.Text, sql));
        }

        public static T CreateEntity<T>(MySqlConnection conn, string sql)
        {
            return CreateEntity<T>(MySqlDbHelper.ExecuteReader(conn, CommandType.Text, sql));
        }


        public static T CreateEntity<T>(string sql, System.Data.Common.DbTransaction dbtran)
        {
            return CreateEntity<T>(DbHelper.ExecuteReader(dbtran, System.Data.CommandType.Text, sql));
        }

        public static List<JSONObject> GetList(string sql)
        {
            List<JSONObject> list = new List<JSONObject>();

            System.Data.DataSet ds = DbHelper.ExecuteDataset(sql);

            if (ds.Tables.Count == 0) return list;

            List<string> fieldList = new List<string>();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                fieldList.Add(ds.Tables[0].Columns[i].ColumnName);
            }

            foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
            {
                JSONObject row = new JSONObject();

                foreach (var field in fieldList)
                {
                    row.Add(field, dataRow[field]);
                }

                list.Add(row);
            }


            return list;
        }

        public static List<Dictionary<string, object>> GetList2(string sql)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            System.Data.DataSet ds = DbHelper.ExecuteDataset(sql);

            if (ds.Tables.Count == 0) return list;

            List<string> fieldList = new List<string>();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                fieldList.Add(ds.Tables[0].Columns[i].ColumnName);
            }

            foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();

                foreach (var field in fieldList)
                {
                    row.Add(field, dataRow[field]);
                }

                list.Add(row);
            }


            return list;
        }

        /// <summary>
        /// added by lt 2016.4.7 增加另一库读法
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> GetList2(System.Data.Common.DbConnection conn, string sql)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            System.Data.DataSet ds = DbHelper.ExecuteDataset(conn, CommandType.Text, sql);

            if (ds.Tables.Count == 0) return list;

            List<string> fieldList = new List<string>();
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                fieldList.Add(ds.Tables[0].Columns[i].ColumnName);
            }

            foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();

                foreach (var field in fieldList)
                {
                    row.Add(field, dataRow[field]);
                }

                list.Add(row);
            }


            return list;
        }

        /// <summary>
        /// added by xuc 2015.08.10 根据sql 获取list entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(string sql)
        {
            List<T> list = new List<T>();

            using (IDataReader reader = DbHelper.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    list.Add(CreateEntityByReader<T>(reader));
                }
            }

            return list;
        }

        /// <summary>
        /// by xie 2015.12.31 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> GetList<T>(System.Data.Common.DbConnection conn, string sql)
        {
            List<T> list = new List<T>();

            //added by xuc 2016.11.16  判断链接类型，调用特定数据库类型dbhelper
            if (conn is MySqlConnection)
            {
                using (IDataReader reader = MySqlDbHelper.ExecuteReader(conn, CommandType.Text, sql))
                {
                    while (reader.Read())
                    {
                        list.Add(CreateEntityByReader<T>(reader));
                    }
                }
            }
            else
            {
                using (IDataReader reader = DbHelper.ExecuteReader(conn, CommandType.Text, sql))
                {
                    while (reader.Read())
                    {
                        list.Add(CreateEntityByReader<T>(reader));
                    }
                }
            }
            return list;
        }




        /// <summary>
        /// 根据reader返回实体类，reader不使用using进行关闭释放 modified by xuc 2015.08.11  
        /// </summary>
        /// <typeparam name="T">泛型类</typeparam>
        /// <param name="dr">datareader对象</param>
        /// <returns>返回T类型实例</returns>
        private static T CreateEntityByReader<T>(System.Data.IDataReader dr)
        {
            T t = Activator.CreateInstance<T>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                Type type = pi.PropertyType;

                if (!ReaderExistsField(dr, pi.Name)) continue;

                if (type.FullName == "System.UInt32")
                {
                    pi.SetValue(t, Convert.ToUInt32(dr[pi.Name]), null);
                }
                if (type.FullName == "System.Guid")
                {
                    pi.SetValue(t, new Guid(dr[pi.Name].ToString()), null);
                }
                else
                {
                    if (DBNull.Value == dr[pi.Name])
                    {
                        pi.SetValue(t, null, null);
                    }
                    else
                    {
                        pi.SetValue(t, dr[pi.Name], null);
                    }
                }
            }

            return t;
        }

        private static bool ReaderExistsField(System.Data.IDataReader dr, string columnName)
        {
            int count = dr.FieldCount;
            for (int i = 0; i < count; i++)
            {
                //modified by xuc 2016.02.24  修复大小写敏感情况，调整为忽略大小写
                if (dr.GetName(i).Equals(columnName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #region 年级

        private static Dictionary<int, string> m_GradeDict = new Dictionary<int, string>();
        public static Dictionary<int, string> GradeDict
        {
            get
            {
                if (m_GradeDict.Count == 0)
                {
                    m_GradeDict[3] = "高三";
                    m_GradeDict[2] = "高二";
                    m_GradeDict[1] = "高一";
                }
                return m_GradeDict;
            }
        }
        public static string GetGradeName(int grade)
        {
            if (GradeDict.ContainsKey(grade))
            {
                return GradeDict[grade];
            }
            else
            {
                return "";
            }
        }

        public static int GetGradeID(string gradeName)
        {
            if (GradeDict.ContainsValue(gradeName))
            {
                foreach (var item in GradeDict)
                {
                    if (item.Value == gradeName)
                    { return item.Key; }
                }
            }
            return 0;
        }

        #endregion

        #region 申请状态

        private static List<ItemInfo> m_ApplyStatusSearchList = new List<ItemInfo>();
        public static List<ItemInfo> ApplyStatusSearchList
        {
            get
            {
                if (m_ApplyStatusSearchList.Count == 0)
                {
                    m_ApplyStatusSearchList.Add(new ItemInfo("全部", "99", ""));
                    m_ApplyStatusSearchList.Add(new ItemInfo("待我审核", "97", ""));
                    m_ApplyStatusSearchList.Add(new ItemInfo("待处理", "98", ""));
                    m_ApplyStatusSearchList.Add(new ItemInfo("待审核", "0", "#f60"));
                    m_ApplyStatusSearchList.Add(new ItemInfo("退回", "-1", "red"));
                    m_ApplyStatusSearchList.Add(new ItemInfo("审核中", "1", "blue"));
                    m_ApplyStatusSearchList.Add(new ItemInfo("审批通过", "2", "green"));
                }
                return m_ApplyStatusSearchList;
            }
        }
        public static ItemInfo GetApplyStatusSearch(int applyStatus)
        {
            ItemInfo info = ApplyStatusSearchList.Find(p => p.Value == applyStatus.ToString());
            if (info != null)
            {
                return info;
            }
            else
            {
                return new ItemInfo();
            }
        }

        #endregion

        #region 枚举转化为select
        public static List<SelectListItem> GetEnumSource<TEnum>()
        {
            var result = (from int item in Enum.GetValues(typeof(TEnum)) let name = Enum.GetName(typeof(TEnum), item) let value = item.ToString() select new SelectListItem { Text = name, Value = value }).ToList();

            result.Insert(0, new SelectListItem { Text = "请选择" });
            return result;
        }
        #endregion

        public static string GetBirthDay(int type, int year, int month, int day)
        {
            if (type == 1)
            {
                return year + "-" + month + "-" + day;
            }
            else if (type == 2)
            {
                ChineseLunisolarCalendar clc = new ChineseLunisolarCalendar();
                DateTime dt = clc.ToDateTime(year, month, day, 0, 0, 0, 0);

                LunarDate lunar = new LunarDate(dt);

                return lunar.ToString();
            }
            return "";
        }

        public static DateTime GetNextBirthday(int birthdayType, int year, int month, int day)
        {
            DateTime dt = DateTime.Now.AddYears(1);
            bool isleap = false;
            int lmonth = month;

            if (birthdayType == 1)
            {
                dt = Convert.ToDateTime(year + "-" + month + "-" + day);
            }
            else if (birthdayType == 2)
            {
                ChineseLunisolarCalendar clc = new ChineseLunisolarCalendar();
                int leapMonth = clc.GetLeapMonth(year);

                //获取今年闰月， 0 则表示没有闰月    
                if (leapMonth > 0)
                {
                    if (leapMonth == month)
                    {
                        isleap = true;
                        lmonth--;
                    }
                    else if (month > leapMonth)
                    {
                        lmonth--;
                    }
                }

                try
                {
                    dt = clc.ToDateTime(year, month, day, 0, 0, 0, 0);
                }
                catch
                {
                    dt = clc.ToDateTime(2000, 1, 1, 0, 0, 0, 0);
                    return dt;
                }
            }
            else
            {
                return Lib.GetSmallDate(DateTime.MinValue);
            }

            if (dt.AddMonths(1) < DateTime.Now)
            {
                return GetNextBirthday(birthdayType, year + 1, lmonth, day, isleap);
            }
            return dt;
        }

        public static DateTime GetNextBirthday(int birthdayType, int year, int month, int day, bool isleap)
        {
            DateTime dt = DateTime.Now.AddYears(1);
            if (birthdayType == 1)
            {
                dt = Convert.ToDateTime(year + "-" + month + "-" + day);
            }
            else if (birthdayType == 2)
            {
                ChineseLunisolarCalendar clc = new ChineseLunisolarCalendar();
                int leapMonth = clc.GetLeapMonth(year);
                // 获取今年闰月， 0 则表示没有闰月

                if (isleap)
                {
                    if (leapMonth == month)
                    {
                        dt = clc.ToDateTime(year, month, day, 0, 0, 0, 0);
                    }
                    else
                    {
                        return GetNextBirthday(birthdayType, year + 1, month, day, isleap);
                    }
                }
                else
                {
                    if (leapMonth > month)
                    {
                        int days = clc.GetDaysInMonth(year + 1, month);
                        if (day > days)
                        {
                            return GetNextBirthday(birthdayType, year + 1, month, day, isleap);
                        }
                        else
                        {
                            dt = clc.ToDateTime(year, month, day, 0, 0, 0, 0);
                        }
                    }
                    else
                    {
                        // 如：1968 八月 三十， 1969年没有，则再加一年
                        if (leapMonth > 0)
                        {
                            int days = clc.GetDaysInMonth(year, month + 1);
                            if (day > days)
                            {
                                return GetNextBirthday(birthdayType, year + 1, month, day, isleap);
                            }
                            else
                            {
                                dt = clc.ToDateTime(year, month + 1, day, 0, 0, 0, 0);
                            }
                        }
                        else
                        {
                            int days = clc.GetDaysInMonth(year, month);
                            if (day > days)
                            {
                                return GetNextBirthday(birthdayType, year + 1, month, day, isleap);
                            }
                            else
                            {
                                dt = clc.ToDateTime(year, month, day, 0, 0, 0, 0);
                            }
                        }
                    }
                }
            }

            if (dt.AddMonths(1) < DateTime.Now)
            {
                return GetNextBirthday(birthdayType, year + 1, month, day, isleap);
            }
            return dt;
        }

        public static string ConvertMoney(decimal Money)
        {
            //金额转换程序  
            string MoneyNum = "";//记录小写金额字符串[输入参数]  
            string MoneyStr = "";//记录大写金额字符串[输出参数]  
            string BNumStr = "零壹贰叁肆伍陆柒捌玖";//模  
            string UnitStr = "万仟佰拾亿仟佰拾万仟佰拾圆角分";//模  

            MoneyNum = ((long)(Money * 100)).ToString();
            for (int i = 0; i < MoneyNum.Length; i++)
            {
                string DVar = "";//记录生成的单个字符(大写)  
                string UnitVar = "";//记录截取的单位  
                for (int n = 0; n < 10; n++)
                {
                    //对比后生成单个字符(大写)  
                    if (Convert.ToInt32(MoneyNum.Substring(i, 1)) == n)
                    {
                        DVar = BNumStr.Substring(n, 1);//取出单个大写字符  
                        //给生成的单个大写字符加单位  
                        UnitVar = UnitStr.Substring(15 - (MoneyNum.Length)).Substring(i, 1);
                        n = 10;//退出循环  
                    }
                }
                //生成大写金额字符串  
                MoneyStr = MoneyStr + DVar + UnitVar;
            }
            //二次处理大写金额字符串  
            MoneyStr = MoneyStr + "整";
            while (MoneyStr.Contains("零分") || MoneyStr.Contains("零角") || MoneyStr.Contains("零佰") || MoneyStr.Contains("零仟")
                || MoneyStr.Contains("零万") || MoneyStr.Contains("零亿") || MoneyStr.Contains("零零") || MoneyStr.Contains("零圆")
                || MoneyStr.Contains("亿万") || MoneyStr.Contains("零整") || MoneyStr.Contains("分整"))
            {
                MoneyStr = MoneyStr.Replace("零分", "零");
                MoneyStr = MoneyStr.Replace("零角", "零");
                MoneyStr = MoneyStr.Replace("零拾", "零");
                MoneyStr = MoneyStr.Replace("零佰", "零");
                MoneyStr = MoneyStr.Replace("零仟", "零");
                MoneyStr = MoneyStr.Replace("零万", "万");
                MoneyStr = MoneyStr.Replace("零亿", "亿");
                MoneyStr = MoneyStr.Replace("亿万", "亿");
                MoneyStr = MoneyStr.Replace("零零", "零");
                MoneyStr = MoneyStr.Replace("零圆", "圆零");
                MoneyStr = MoneyStr.Replace("零整", "整");
                MoneyStr = MoneyStr.Replace("分整", "分");
            }
            if (MoneyStr == "整")
            {
                MoneyStr = "零元整";
            }
            return MoneyStr;
        }

        public static Dictionary<string, object> GetHtml(string url, Dictionary<string, string> dict)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            //wc.Proxy = null;
            //ServicePointManager.DefaultConnectionLimit = 500;
            wc.Encoding = Encoding.UTF8;

            string postdata = "";
            foreach (var item in dict)
            {
                postdata += "&" + item.Key + "=" + System.Web.HttpUtility.UrlEncode(item.Value);
            }

            LogManager.WriteLog(LogFile.Html, url + "?" + postdata.Trim('&'));

            string str = wc.DownloadString(url + "?" + postdata.Trim('&'));


            LogManager.WriteLog(LogFile.Html, str);

            Dictionary<string, object> json = JsonHelper.DeserializeObject<Dictionary<string, object>>(str);

            return json;
        }
    }

    public enum MemberLevel
    {
        Guest = 0,
        RegMember = 1,
        General = 2,
        Gold = 3,
        Vip = 4,
        SVip = 5
    }

}