namespace OMTB.Dms.Data.Entity
{
    /// <summary>
    /// Rds实例信息
    /// </summary>
    public class RdsDbInstanceInfoRepo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 实例名
        /// </summary>
        public string DBInstanceId { get; set; }
        
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        
        /// <summary>
        /// 数据库名称
        /// </summary>
        /// <returns></returns>
        public string DbName { get; set; }
    }
}