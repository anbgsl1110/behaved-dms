using OMTB.Component.Util;
using OMTB.Dms.Common;

namespace OMTB.Dms.Dto.Model
{
    /// <summary>
    /// 通过应用名称获取慢日志请求对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByAppNameRequest : DescriptionRequest
    {
        public DescriptionGetSqlSlowLogByAppNameRequest() 
            : base("GetSqlSlowLogByAppName.")
        {
        }
        
        private string _appName;
        private int _dateRange;
        
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName {
            get { return _appName; }
            set
            {
                _appName = value;
                DictionaryUtil.Add(QueryParameters,ActionName + "AppName",value);
            }
        }

        /// <summary>
        /// 日期范围
        /// </summary>
        public int DateRange
        {
            get { return this._dateRange; }
            set
            {
                _dateRange = value;
                DictionaryUtil.Add(QueryParameters,ActionName + "DateRange",value);
            }
        }
    }
}