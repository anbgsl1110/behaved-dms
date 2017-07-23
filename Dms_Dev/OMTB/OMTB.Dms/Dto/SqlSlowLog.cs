using System;

namespace OMTB.Dms.Dto
{
    /// <summary>
    /// 阿里云返回慢日志对象
    /// </summary>
    public class SqlSlowLog
    {
        /// <summary>
        /// DB名称
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        public string SQLText { get; set; }

        /// <summary>
        /// 执行总次数
        /// </summary>
        public long MySQLTotalExecutionCounts { get; set; }

        /// <summary>
        /// 执行总时长，单位：秒
        /// </summary>
        public long MySQLTotalExecutionTimes { get; set; }

        /// <summary>
        /// 执行最大时长，单位：秒
        /// </summary>
        public long MaxExecutionTime { get; set; }

        /// <summary>
        /// 锁定总时长，单位：秒
        /// </summary>
        public long TotalLockTimes { get; set; }

        /// <summary>
        /// 锁定最大时长，单位：秒
        /// </summary>
        public long MaxLockTime { get; set; }

        /// <summary>
        /// 解析总行数
        /// </summary>
        public long ParseTotalRowCounts { get; set; }

        /// <summary>
        /// 解析最大行数
        /// </summary>
        public long ParseMaxRowCount { get; set; }

        /// <summary>
        /// 返回总行数
        /// </summary>
        public long ReturnTotalRowCounts { get; set; }

        /// <summary>
        /// 返回最大行数
        /// </summary>
        public long ReturnMaxRowCount { get; set; }

        /// <summary>
        /// 数据生成日期,格式："yyyy-MM-ddZ"，如2011-05-30Z
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}