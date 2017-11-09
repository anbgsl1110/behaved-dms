namespace OMTB.Dms.Dto
{
    /// <summary>
    /// Rds Api查看慢日志请求参数
    /// </summary>
    public class RdsDescribeSlowLogsRequestParameter : RdsApiShareRequestParameter
    {
        /// <summary>
        /// 系统规定参数，取值：DescribeSlowLogs
        /// </summary>
        public string Action { get; set; }
        
        /// <summary>
        /// 实例名
        /// </summary>
        public string DBInstanceId { get; set; }
        
        /// <summary>
        /// 查询开始日期，格式：YYYY-MM-DDZ，如2011-05-30Z
        /// </summary>
        public string StartTime { get; set; }
        
        /// <summary>
        /// 查询结束日期，不能小于查询开始日期，
        /// 格式：YYYY-MM-DDZ，如2011-05-30Z
        /// </summary>
        public string EndTime { get; set; }
        
        /// <summary>
        /// DB名称
        /// </summary>
        public string DBName { get; set; }
        
        /// <summary>
        /// 排序依据，取值：TotalExecutionCounts:总执行次数最多;
        /// TotalQueryTimes:总执行时间最多;
        /// TotalLogicalReads:总逻辑读最多;
        /// TotalPhysicalReads:总物理读最多;
        /// 此参数对SQLServer实例有效，SQLServer类型必传此参数
        /// </summary>
        public string SortKey { get; set; }
        
        /// <summary>
        /// 每页记录数，取值：30/50/100默认值：30
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// 页码，大于0，且不超过Integer的最大值;默认值：1
        /// </summary>
        public int PageNumber { get; set; }
    }
}