using System.ComponentModel.DataAnnotations;

namespace OMTB.Dms.Web.Models
{  
    public class GetSqlExcuteResultViewModel
    {
        /// <summary>
        /// 执行Sql字符串
        /// </summary>
        [Required]
        public string SqlString { get; set; }
        
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        [Required]
        public string DbConnectName { get; set; }
    }

    public class ExportSqlExcuteResultToExcelViewModel
    {
        /// <summary>
        /// 执行Sql字符串
        /// </summary>
        [Required]
        public string SqlString { get; set; }
        
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        [Required]
        public string DbConnectName { get; set; }
    }
}