using System;
using OMTB.Dms.Dto;

namespace OMTB.Dms.Data.Entity
{
    /// <summary>
    /// Rds慢日志请求
    /// </summary>
    public class RdsSlowLogRequestRepo : RdsDescribeSlowLogsResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 请求时间
        /// </summary>
        public DateTime RequestTime { get; set; }
    }
}