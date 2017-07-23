using System;

namespace OMTB.Dms.Dto.Model
{
    /// <summary>
    /// 通过筛选参数获取慢日志请求对象
    /// </summary>
    public class DescriptionGetSqlSlowLogByParameterResponse : DescriptionResponse
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public string StarTime { get; set; }
        
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public string EndTime { get; set; }
    }
}