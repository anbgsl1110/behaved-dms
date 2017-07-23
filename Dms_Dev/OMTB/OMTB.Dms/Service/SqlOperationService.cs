using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Greedy.Dapper;
using OMTB.Component.Excel;
using OMTB.Component.Util;
using OMTB.Dms.Common;
using OMTB.Dms.Dto;
using OMTB.Dms.IService;
//using Dapper;

namespace OMTB.Dms.Service
{
    /// <summary>
    /// SQL操作服务
    /// </summary>
    public class SqlOperationService : BaseService,ISqlOperationService
    {
        public ServiceResult<ServiceStateEnum, IEnumerable<dynamic>> GetSqlExcuteResult(string sql,
            DbconnectInfo dbconnectInfo)
        {
            //参数检查
            var verityResult = VeritySqlExcuteParameter(sql, dbconnectInfo);
            if (!verityResult.Success)
            {
                return ServiceResult.Create(false, verityResult.State,default(IEnumerable<dynamic>));
            }
            
            //结果查询
            using (var dbConnection = DbFactory.NewConnection(dbconnectInfo))
            {
                try
                {
                    var result = dbConnection.Query(sql);
                    if (result.Count() > ConfigHelper.GetConfigInt("MaxSelcetAmount"))
                    {
                        return ServiceResult.Create(false, ServiceStateEnum.OverSelcetAmountLimit,
                            default(IEnumerable<dynamic>));
                    }
                    return ServiceResult.Create(true, ServiceStateEnum.Success, result);
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                }
                return ServiceResult.Create(false, ServiceStateEnum.SqlSyntaxError, default(IEnumerable<dynamic>));
            }
        }

        public ServiceResult<ServiceStateEnum, DataTable> GetExportResult(string sql,
            DbconnectInfo dbconnectInfo)
        {
            //参数检查
            var verityResult = VeritySqlExcuteParameter(sql, dbconnectInfo);
            if (!verityResult.Success)
            {
                return ServiceResult.Create(false, ServiceStateEnum.NoDataExported, default(DataTable));
            }

            //结果查询
            using (var dbConnection = DbFactory.NewConnection(dbconnectInfo))
            {
                try
                {
                    var dataReader = dbConnection.ExecuteReader(sql);
                    var dataTable = DataTableHelper.ConvertDataReaderToDataTable(dataReader);
                    if (dataTable.Rows.Count > ConfigHelper.GetConfigInt("MaxSelcetAmount"))
                    {
                        return ServiceResult.Create(false, ServiceStateEnum.OverSelcetAmountLimit, default(DataTable));
                    }
                    return ServiceResult.Create(true, ServiceStateEnum.Success, dataTable);
                }
                catch (Exception)
                {
                    //Console.WriteLine(e.Message);
                }
                return ServiceResult.Create(false, ServiceStateEnum.NoDataExported, default(DataTable));
            }
        }

        public ServiceResult<ServiceStateEnum, Stream> ExportSqlExcuteResultToExcel(DataTable dataTable)
        {
            var excelStream = DataImportExcel.DataTableToDefaultStyleExcel(dataTable);
            return ServiceResult.Create(true, ServiceStateEnum.Success, excelStream);
        }

        #region 私有方法

        /// <summary>
        /// 验证Sql执行参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dbconnectInfo"></param>
        /// <returns></returns>
        private ServiceResult<ServiceStateEnum, bool> VeritySqlExcuteParameter(string sql, DbconnectInfo dbconnectInfo)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                return ServiceResult.Create(false, ServiceStateEnum.SqlStringNotNullOrWhite, false);
            }
            if (dbconnectInfo == null)
            {
                return ServiceResult.Create(false, ServiceStateEnum.ParameterNotNull, false);
            }
            //判断是否包含限制命令语法敏感词
            ConfigService configService = new ConfigService();
            List<string> list = configService.GetSqlSensitiveWordsList().Data;
            if (list.Any())
            {
                var sqlString = sql.Trim().ToUpper();
                foreach (var str in list)
                {
                    if (sqlString.Contains(str))
                    {
                        return ServiceResult.Create(false, ServiceStateEnum.ContainLimitCommand, false);
                    }
                }               
            }
            return ServiceResult.Create(true, ServiceStateEnum.Success, true);
        }

        #endregion
    }
}