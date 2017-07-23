namespace OMTB.Dms.Data.Entity
{
    /// <summary>
    /// 配置信息存储对象
    /// </summary>
    public class ConfigInfoRepo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; }

        /// <summary>
        /// 配置备注
        /// </summary>
        public string ConfigRemark { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        public string ConfigDescription { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValidate { get; set; }

        /// <summary>
        /// 是否允许修改
        /// </summary>
        public bool IsModify { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
