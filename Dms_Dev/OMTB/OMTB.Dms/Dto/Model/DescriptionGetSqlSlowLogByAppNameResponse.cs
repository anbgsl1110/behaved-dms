using OMTB.Dms.Common;

namespace OMTB.Dms.Dto.Model
{
    /// <summary>
    /// 通过应用名称获取慢日志参数转化解组响应对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByAppNameResponse : DescriptionResponse
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 日期范围
        /// </summary>
        public DateRangeEnum? DateRange { get; set; }
    }
}