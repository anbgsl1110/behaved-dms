using System.Collections.Generic;
using System.Data;
using System.IO;
using OMTB.Dms.Common;
using OMTB.Dms.Dto;

namespace OMTB.Dms.IService
{
    /// <summary>
    /// Sql执行服务接口
    /// </summary>
    public interface ISqlOperationService
    {
        /// <summary>
        /// 获取Sql执行返回结果集
        /// </summary>
        /// <param name="sql">执行的sql字符串</param>
        /// <param name="dbconnectInfo">数据库连接信息</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, IEnumerable<dynamic>> GetSqlExcuteResult(string sql,
            DbconnectInfo dbconnectInfo);

        /// <summary>
        /// 获取导出结果集
        /// </summary>
        /// <param name="sql">执行的sql字符串</param>
        /// <param name="dbconnectInfo">数据库连接信息</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, DataTable> GetExportResult(string sql, DbconnectInfo dbconnectInfo);

        /// <summary>
        /// 导出Sql执行返回结果到Excel
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, Stream> ExportSqlExcuteResultToExcel(DataTable dataTable);
    }
}