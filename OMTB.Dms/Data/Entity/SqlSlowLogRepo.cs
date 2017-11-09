using OMTB.Dms.Dto;

namespace OMTB.Dms.Data.Entity
{
    /// <summary>
    /// Mysql慢日志
    /// </summary>
    public class SqlSlowLogRepo : SqlSlowLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// app应用名称，如：校宝
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 实例名
        /// </summary>
        public string DBInstanceId { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public string SQLSlowLogRequestId { get; set; }
    }
}