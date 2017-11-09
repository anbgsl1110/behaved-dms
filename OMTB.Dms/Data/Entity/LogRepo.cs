using System;

namespace OMTB.Dms.Data.Entity
{
    /// <summary>
    /// 日志存储对象
    /// </summary>
    public class LogRepo
    {
        /// <summary>
        /// 日志主键
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperationContext { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public string OperationResult { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationDate { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}