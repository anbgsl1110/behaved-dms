namespace OMTB.Dms.Dto
{
    /// <summary>
    /// 数据库连接信息
    /// </summary>
    public class DbconnectInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        
                
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        public string DbConnectName { get; set; }
        
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        /// 数据库驱动提供信息
        /// </summary>
        public string ProviderNameString { get; set; }
    }
}