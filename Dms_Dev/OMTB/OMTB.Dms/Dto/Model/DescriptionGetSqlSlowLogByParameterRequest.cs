using System;
using OMTB.Component.Util;

namespace OMTB.Dms.Dto.Model
{
    /// <summary>
    /// 通过筛选参数获取慢日志参数转化解组响应对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByParameterRequest : DescriptionRequest
    {
        public DescriptionGetSqlSlowLogByParameterRequest() : base("GetSqlSlowLogByParameter.")
        {
        }

        private string _appName;
        private string _startTime;
        private string _endTime;

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName
        {
            get { return _appName; }
            set
            {
                _appName = value;
                DictionaryUtil.Add(QueryParameters, ActionName + "AppName", value);
            }
        }

        /// <summary>
        /// 查询开始时间
        /// </summary>
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                DictionaryUtil.Add(QueryParameters, ActionName + "StartTime", value);
            }
        }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public string EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                DictionaryUtil.Add(QueryParameters, ActionName + "EndTime", value);
            }
        }
    }
}