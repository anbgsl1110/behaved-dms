using System.Collections.Generic;
using System.Data;
using System.IO;
using OMTB.Dms.Common;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;

namespace OMTB.Dms.IService
{
    /// <summary>
    /// Rds慢日志服务接口
    /// </summary>
    public interface IRdsSlowLogService
    {
        /// <summary>
        /// 增加Rds慢日志请求
        /// </summary>
        /// <param name="rdsSlowLogRequestRepo">Rds慢日志请求对象</param>
        void AddRdsSlowLogRequestRepo(RdsSlowLogRequestRepo rdsSlowLogRequestRepo);

        /// <summary>
        /// 增加慢日志
        /// </summary>
        /// <param name="sqlSlowLogRepo">Mysql慢日志</param>
        void AddSqlSlowLogRepo(SqlSlowLogRepo sqlSlowLogRepo);

        /// <summary>
        /// 按慢日志集合批量增加慢日志
        /// </summary>
        /// <param name="sqlSlowLogRepoList">Mysql慢日志列表</param>
        void AddSqlSlowLogRepoList(List<SqlSlowLogRepo> sqlSlowLogRepoList);

        /// <summary>
        /// Dms根据应用名称获取查询慢日志整合数据
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="dateRangeEnum">请求日期范围枚举</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<SqlSlowLog>> GetSqlSlowLogByAppName(string appName, int dateRangeEnum);

        /// <summary>
        /// 根据筛选参数获取查询慢日志整合数据
        /// </summary>
        /// <returns>查询筛选参数字典</returns>
        ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> GetSqlSlowLogByParameter(
            IDictionary<string, object> parameter);

        /// <summary>
        /// 获取慢日志查询结果导出数据
        /// </summary>
        /// <param name="parameter">查询筛选参数字典</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> GetExportSqlSlowLogResult(
            IDictionary<string, object> parameter);

        /// <summary>
        /// 导出慢日志查询结果到Excel
        /// </summary>
        /// <param name="dataTable">导出数据Datatable对象</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, Stream> ExportSqlSlowLogResultToExcel(DataTable dataTable);
    }
}