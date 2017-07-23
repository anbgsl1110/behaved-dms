using OMTB.Dms.Common;

namespace OMTB.Dms.Web.Models
{
    /// <summary>
    /// 获取Rds慢日志查询结构前端请求对象 
    /// </summary>
    public class GetRdsSelectResultRequest
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
            
        /// <summary>
        /// 查询日期范围
        /// </summary>
        public string DateRange { get; set; }
    }
    
    /// <summary>
    /// 导出慢日志查询结果到Excel前端请求对象
    /// </summary>
    public class ExportSqlSlowLogResultToExcelRequest
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 查询日期范围
        /// </summary>
        public string DateRange { get; set; }
    }
}