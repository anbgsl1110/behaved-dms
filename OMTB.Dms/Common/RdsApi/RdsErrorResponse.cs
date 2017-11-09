namespace OMTB.Dms.Common.RdsApi
{
    /// <summary>
    /// Rds请求失败时返回对象
    /// </summary>
    public class RdsErrorResponse
    {
        /// <summary>
        /// 全局唯一的请求ID
        /// </summary>
        public string RequestId { get; set; }
        
        /// <summary>
        /// 该次请求访问的站点ID
        /// </summary>
        public string HostId { get; set; }
        
        /// <summary>
        /// 错误代码
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}