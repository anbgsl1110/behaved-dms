using System.Collections.Generic;

namespace DbDocGenerate.Data
{
    public class DataListResult<T>
    {
        public int RecordCount { get; set; }
        public List<T> List { get; set; }
        public DataListResult()
        {
            List = new List<T>();
        }
    }

    public class DataListResult1<T>
    {
        public string TimeStamp { get; set; }
        public List<T> List { get; set; }
        public DataListResult1()
        {
            List = new List<T>();
        }
    }

    public class DataListInfo
    {
        public int RecordCount { get; set; }

        public List<Dictionary<string, object>> List { get; set; }

        public DataListInfo()
        {
            List = new List<Dictionary<string, object>>();
        }
    }

    public class PagerInfo
    {
        private int _PageSize = 20;     // 默认分页数
        private int _CurrentPage = 0;   // 当前页

        public int CurrentPage { get { return _CurrentPage; } set { _CurrentPage = value; } }

        public int PageSize { get { return _PageSize; } set { _PageSize = value; } }

        public string OrderField { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        public Dictionary<string, object> WhereDict { get; set; }
    }

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderTypeEnum
    {
        Asc = 1,
        Desc
    }
}
