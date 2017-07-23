using System;
using System.Collections.Generic;

namespace OMTB.Dms.Dto
{
    /// <summary>
    /// Rds慢日志请求返回对象
    /// </summary>
    public class RdsDescribeSlowLogsResponse : DescriptionResponse
    {        
        /// <summary>
        /// 实例名
        /// </summary>
        public string DBInstanceID { get; set; }
        
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string Engine { get; set; }
        
        /// <summary>
        /// 查询开始日期，格式：YYYY-MM-DDZ，如2011-05-30Z
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 查询结束日期，格式：YYYY-MM-DDZ，如2011-05-30Z
        /// </summary>
        public DateTime EndTime { get; set; }
        
        /// <summary>
        /// 总记录数
        /// </summary>
        public long TotalRecordCount { get; set; }
        
        /// <summary>
        /// 页码
        /// </summary>
        public long PageNumber { get; set; }
        
        /// <summary>
        /// 本页SQL语句个数
        /// </summary>
        public long PageRecordCount { get; set; }
    }

    /// <summary>
    /// Rds慢日志列表成功请求返回对象
    /// </summary>
    public class RdsDescribeSlowLogsSuccessResponse : RdsDescribeSlowLogsResponse
    {       
        /// <summary>
        /// 慢日志列表属性
        /// </summary>
        public SqlSlowLogList Items{ get; set; }

        /// <summary>
        /// 慢日志列表对象
        /// </summary>
        public class SqlSlowLogList
        {
            public List<SqlSlowLog> SQLSlowLog { get; set; }
        }
    }
}