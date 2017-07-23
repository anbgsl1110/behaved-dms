using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OMTB.Component.Excel;
using OMTB.Component.Transform;
using OMTB.Component.Util;
using OMTB.Dms.Common;
using OMTB.Dms.Common.RdsApi;
using OMTB.Dms.Data.Database;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;
using OMTB.Dms.Dto.Model;
using OMTB.Dms.Dto.Transform;
using OMTB.Dms.IService;

namespace OMTB.Dms.Service
{
    /// <summary>
    /// Rds慢日志服务
    /// </summary>
    public class RdsSlowLogService : BaseService, IRdsSlowLogService
    {
        public void AddRdsSlowLogRequestRepo(RdsSlowLogRequestRepo rdsSlowLogRequestRepo)
        {
            using (DmsDbContext dmsDbContext = new DmsDbContext())
            {
                if (rdsSlowLogRequestRepo != null)
                {
                    dmsDbContext.RdsSlowLogRequest.Add(rdsSlowLogRequestRepo);
                    dmsDbContext.SaveChanges();
                }
            }
        }

        public void AddSqlSlowLogRepo(SqlSlowLogRepo sqlSlowLogRepo)
        {
            using (DmsDbContext dmsDbContext = new DmsDbContext())
            {
                if (sqlSlowLogRepo != null)
                {
                    dmsDbContext.SqlSlowLog.Add(sqlSlowLogRepo);
                    dmsDbContext.SaveChanges();
                }
            }
        }

        public void AddSqlSlowLogRepoList(List<SqlSlowLogRepo> sqlSlowLogRepoList)
        {
            using (DmsDbContext dmsDbContext = new DmsDbContext())
            {
                if (sqlSlowLogRepoList.Any())
                {
                    foreach (var slowLog in sqlSlowLogRepoList)
                    {
                        if (slowLog != null)
                        {
                            dmsDbContext.SqlSlowLog.Add(slowLog);
                        }
                    }
                }
                dmsDbContext.SaveChanges();
            }
        }

        #region 获取慢日志数据

        #region Dms根据应用名称获取查询慢日志整合数据

        public ServiceResult<ServiceStateEnum, List<SqlSlowLog>> GetSqlSlowLogByAppName(string appName,
            int dateRangeEnum)
        {
            //获取改Action请求参数转换后的对象
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse =
                GetSqlSlowLogByAppNameRequestUnmarshaller(appName, dateRangeEnum);

            using (DmsDbContext dmsDbContext = new DmsDbContext())
            {
                //获取前一天的数据时可以从数据库取
                if (descriptionGetSqlSlowLogResponse.DateRange == DateRangeEnum.OneDaY)
                {
                    DateTime startTime = DateTime.Today.AddDays(-1);
                    var result = dmsDbContext.SqlSlowLog
                        .Where(p => p.Id > 0 && p.AppName.Equals(descriptionGetSqlSlowLogResponse.AppName) &&
                                    p.CreateTime == startTime).ToList();
                    //数据库存在数据直接从数据库获取数据
                    if (result.Any())
                    {
                        List<SqlSlowLog> slowLogs = new List<SqlSlowLog>();
                        foreach (var slowLogRepo in result)
                        {
                            if (slowLogRepo != null)
                            {
                                slowLogs.Add(new SqlSlowLog
                                {
                                    DBName = slowLogRepo.DBName,
                                    SQLText = slowLogRepo.SQLText,
                                    MySQLTotalExecutionCounts = slowLogRepo.MySQLTotalExecutionCounts,
                                    MySQLTotalExecutionTimes = slowLogRepo.MySQLTotalExecutionTimes,
                                    MaxExecutionTime = slowLogRepo.MaxExecutionTime,
                                    TotalLockTimes = slowLogRepo.TotalLockTimes,
                                    MaxLockTime = slowLogRepo.MaxLockTime,
                                    ParseTotalRowCounts = slowLogRepo.ParseTotalRowCounts,
                                    ParseMaxRowCount = slowLogRepo.ParseMaxRowCount,
                                    ReturnTotalRowCounts = slowLogRepo.ReturnTotalRowCounts,
                                    ReturnMaxRowCount = slowLogRepo.ReturnMaxRowCount,
                                    CreateTime = slowLogRepo.CreateTime
                                });
                            }
                        }
                        return ServiceResult.Create(true, ServiceStateEnum.Success, slowLogs);
                    }
                }

                //数据库不存在数据时尝试通过访问阿里云Rds的Api获取数据并写入数据库
                ServiceResult<ServiceStateEnum, List<SqlSlowLog>> getDataByApiResult =
                    GetSqlSlowLogByRdsApiAndWriteToDatabase(descriptionGetSqlSlowLogResponse);

                return getDataByApiResult;
            }
        }

        /// <summary>
        /// 获取[Rds根据应用名称获取查询慢日志整合数据]Action请求参数转换后的对象
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <param name="dateRangeEnum">请求日期范围枚举</param>
        /// <returns></returns>
        private DescriptionGetSqlSlowLogByAppNameResponse GetSqlSlowLogByAppNameRequestUnmarshaller(string appName,
            int dateRangeEnum)
        {
            DescriptionGetSqlSlowLogByAppNameRequest request = new DescriptionGetSqlSlowLogByAppNameRequest();
            request.AppName = appName;
            request.DateRange = dateRangeEnum;
            UnmarshallerContext unmarshallerContext = new UnmarshallerContext
            {
                ResponseDictionary = request.QueryParameters
            };
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse =
                DescriptionGetSqlSlowLogByAppNameUmarshaller.Unmarshall(unmarshallerContext);

            return descriptionGetSqlSlowLogResponse;
        }

        /// <summary>
        /// 通过访问阿里云Rds的Api获取慢日志整合数据并写入数据到数据库
        /// </summary>
        /// <param name="descriptionGetSqlSlowLogResponse">通过应用名称获取慢日志参数转化解组响应对象</param>
        /// <returns></returns>
        private ServiceResult<ServiceStateEnum, List<SqlSlowLog>> GetSqlSlowLogByRdsApiAndWriteToDatabase(
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse)
        {
            List<SqlSlowLog> slowLogs = new List<SqlSlowLog>();
            List<SqlSlowLogRepo> slowLogRepos = new List<SqlSlowLogRepo>();            
            //获取请求开始和结束时间
            Tuple<string, string> tuple = GetRequestDateInterval(descriptionGetSqlSlowLogResponse);
            //获取该应用下的数据库实例信息
            var getRdsDbInstanceInfoResult =
                ConfigService.GetDbInstanceIdListByAppName(descriptionGetSqlSlowLogResponse.AppName);
            if (!getRdsDbInstanceInfoResult.Data.Any())
            {
                return ServiceResult.Create(false, ServiceStateEnum.ParameterError, slowLogs);
            }
            foreach (var rdsDbInstanceInfo in getRdsDbInstanceInfoResult.Data)
            {
                string dbInstanceId = rdsDbInstanceInfo.DBInstanceId;
                string dbName = rdsDbInstanceInfo.DbName;
                //Get请求获取数据          
                String responseExecute = CreateRequest().Execute("DescribeSlowLogs",
                    new Dictionary<String, String>
                    {
                        {"DBInstanceId", dbInstanceId},
                        {"StartTime", tuple.Item1}, //startTime
                        {"EndTime", tuple.Item2}, //endTime
                        {"DBName", dbName},
                        {"SortKey", "TotalQueryTimes"},
                        {"PageSize", "100"},
                        {"PageNumber", "1"}
                    });
    
                var rdsDescribeSlowLogsSuccessResponse =
                    JsonConvert.DeserializeObject<RdsDescribeSlowLogsSuccessResponse>(responseExecute);
    
                if (rdsDescribeSlowLogsSuccessResponse.Engine == null)
                {
                    return ServiceResult.Create(false, ServiceStateEnum.Failed, slowLogs);
                }
                //数据库增加Rds慢日志请求
                AddRdsSlowLogRequestRepo(new RdsSlowLogRequestRepo
                {
                    DBInstanceID = dbInstanceId,
                    RequestTime = DateTime.Now,
                    PageNumber = rdsDescribeSlowLogsSuccessResponse.PageNumber,
                    TotalRecordCount = rdsDescribeSlowLogsSuccessResponse.TotalRecordCount,
                    RequestId = rdsDescribeSlowLogsSuccessResponse.RequestId,
                    EndTime = rdsDescribeSlowLogsSuccessResponse.EndTime,
                    StartTime = rdsDescribeSlowLogsSuccessResponse.StartTime,
                    Engine = rdsDescribeSlowLogsSuccessResponse.Engine,
                    PageRecordCount = rdsDescribeSlowLogsSuccessResponse.PageRecordCount
                });
                //保存请求结果到数据库对象列表
                SaveResponseDataToRepoList(descriptionGetSqlSlowLogResponse, dbInstanceId,
                    rdsDescribeSlowLogsSuccessResponse,
                    ref slowLogs, ref slowLogRepos);
                //删除已有数据写入获取的数据到Dms数据库
                DeleteExistingWriteSqlSlowLogToDataBase(slowLogRepos, descriptionGetSqlSlowLogResponse);    
            }            
            return ServiceResult.Create(true, ServiceStateEnum.Success, slowLogs);
        }

        /// <summary>
        /// 保存请求结果到数据库对象列表
        /// </summary>
        /// <param name="descriptionGetSqlSlowLogResponse">通过应用名称获取慢日志参数转化解组响应对象</param>
        /// <param name="dbInstanceId">数据库实例Id</param>
        /// <param name="rdsDescribeSlowLogsSuccessResponse">Rds慢日志列表成功请求返回对象</param>
        /// <param name="slowLogs">阿里云返回慢日志对象集合</param>
        /// <param name="slowLogRepos">Mysql慢日志存储对象集合</param>
        private void SaveResponseDataToRepoList(
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse,
            string dbInstanceId, RdsDescribeSlowLogsSuccessResponse rdsDescribeSlowLogsSuccessResponse,
            ref List<SqlSlowLog> slowLogs, ref List<SqlSlowLogRepo> slowLogRepos)
        {
            if (slowLogs == null) throw new ArgumentNullException(nameof(slowLogs));

            var slowLoglist = rdsDescribeSlowLogsSuccessResponse.Items.SQLSlowLog;
            foreach (var slowLog in slowLoglist)
            {
                SqlSlowLogRepo sqlSlowLogRepo = new SqlSlowLogRepo
                {
                    DBInstanceId = dbInstanceId,
                    SQLSlowLogRequestId = rdsDescribeSlowLogsSuccessResponse.RequestId,
                    AppName = descriptionGetSqlSlowLogResponse.AppName,
                    DBName = slowLog.DBName,
                    SQLText = slowLog.SQLText,
                    MySQLTotalExecutionCounts = slowLog.MySQLTotalExecutionCounts,
                    MySQLTotalExecutionTimes = slowLog.MySQLTotalExecutionTimes,
                    MaxExecutionTime = slowLog.MaxExecutionTime,
                    TotalLockTimes = slowLog.TotalLockTimes,
                    MaxLockTime = slowLog.MaxLockTime,
                    ParseTotalRowCounts = slowLog.ParseTotalRowCounts,
                    ParseMaxRowCount = slowLog.ParseMaxRowCount,
                    ReturnTotalRowCounts = slowLog.ReturnTotalRowCounts,
                    ReturnMaxRowCount = slowLog.ReturnMaxRowCount,
                    CreateTime = slowLog.CreateTime
                };
                slowLogRepos.Add(sqlSlowLogRepo);
            }
            slowLogs.AddRange(slowLoglist);
        }

        /// <summary>
        /// 获取请求开始和结束时间
        /// </summary>
        /// <param name="descriptionGetSqlSlowLogResponse">通过应用名称获取慢日志参数转化解组响应对象</param>
        /// <returns></returns>
        private Tuple<string, string> GetRequestDateInterval(
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse)
        {
            string startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
            string endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
            switch (descriptionGetSqlSlowLogResponse.DateRange)
            {
                case DateRangeEnum.OneDaY:
                    startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
                    endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
                    break;
                case DateRangeEnum.ThreeDay:
                    startTime = DateTime.Now.AddDays(-3).ToString("yyyy-MM-ddZ");
                    endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
                    break;
                case DateRangeEnum.SevenDay:
                    startTime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-ddZ");
                    endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
                    break;
            }
            return new Tuple<string, string>(startTime, endTime);
        }

        /// <summary>
        /// 删除已有数据写入获取的数据到Dms数据库
        /// </summary>
        /// <param name="slowLogRepos">Mysql慢日志存储对象集合</param>
        /// <param name="descriptionGetSqlSlowLogResponse">通过应用名称获取慢日志参数转化解组响应对象</param>
        private void DeleteExistingWriteSqlSlowLogToDataBase(List<SqlSlowLogRepo> slowLogRepos,
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse)
        {
            DeteteSqlSlowLogByDataRange(descriptionGetSqlSlowLogResponse);

            AddSqlSlowLogRepoList(slowLogRepos);
        }

        /// <summary>
        /// 删除已有指定天数数据库中的数据
        /// </summary>
        /// <param name="descriptionGetSqlSlowLogResponse">通过应用名称获取慢日志参数转化解组响应对象</param>
        private void DeteteSqlSlowLogByDataRange(
            DescriptionGetSqlSlowLogByAppNameResponse descriptionGetSqlSlowLogResponse)
        {
            using (DmsDbContext dbContext = new DmsDbContext())
            {
                if (descriptionGetSqlSlowLogResponse.DateRange != null)
                {
                    double rangeDays = -(double) descriptionGetSqlSlowLogResponse.DateRange;
                    DateTime startDateTime = DateTime.Today.AddDays(rangeDays-1);
                    DateTime endDateTime = DateTime.Today;
                    var result = dbContext.SqlSlowLog
                        .Where(p => p.Id > 0 && p.AppName.Equals(descriptionGetSqlSlowLogResponse.AppName) &&
                                    p.CreateTime < endDateTime && p.CreateTime > startDateTime).ToList();
                    if (result.Any())
                    {
                        foreach (var slowLogRepo in result)
                        {
                            dbContext.SqlSlowLog.Remove(slowLogRepo);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        #endregion

        #region 根据筛选参数获取查询慢日志整合数据
        
        public ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> GetSqlSlowLogByParameter(
            IDictionary<string, object> parameter)
        {
            // 数据验证
            if (!parameter.ContainsKey(@"AppName") || !parameter.ContainsKey(@"StartTime") ||
                !parameter.ContainsKey("EndTime"))
            {
                return ServiceResult.Create(false, ServiceStateEnum.ParameterError, new List<SqlSlowLogRepo>());
            }
            // 请求参数对象转换            
            DescriptionGetSqlSlowLogByParameterResponse response =
                GetSqlSlowLogByParameterRequestUnmarshaller(parameter);
            // 从阿里云Rds的Api获取数据并写入数据库
            ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> getServiceResult =
                GetSqlSlowLogByParameterRdsApiAndWriteToDatabase(response);
            return getServiceResult;
        }

        /// <summary>
        /// 通过筛选参数从阿里云Rds的Api获取数据并写入数据库
        /// </summary>
        /// <param name="response">通过筛选参数获取慢日志请求对象</param>
        /// <returns></returns>
        private ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> GetSqlSlowLogByParameterRdsApiAndWriteToDatabase(
            DescriptionGetSqlSlowLogByParameterResponse response)
        {
            List<SqlSlowLog> slowLogs = new List<SqlSlowLog>();
            List<SqlSlowLogRepo> slowLogRepos = new List<SqlSlowLogRepo>();
            //获取该应用下的数据库实例信息
            var getRdsDbInstanceInfoResult =
                ConfigService.GetDbInstanceIdListByAppName(response.AppName);
            if (!getRdsDbInstanceInfoResult.Data.Any())
            {
                return ServiceResult.Create(false, ServiceStateEnum.ParameterError, slowLogRepos);
            }
            foreach (var rdsDbInstanceInfo in getRdsDbInstanceInfoResult.Data)
            {           
                string dbInstanceId = rdsDbInstanceInfo.DBInstanceId;
                string dbName = rdsDbInstanceInfo.DbName;
                //Get请求获取数据          
                String responseExecute = CreateRequest().Execute("DescribeSlowLogs",
                    new Dictionary<String, String>
                    {
                        {"DBInstanceId", dbInstanceId},
                        {"StartTime", response.StarTime},
                        {"EndTime", response.EndTime},
                        {"DBName", dbName},
                        {"SortKey", "TotalQueryTimes"},
                        {"PageSize", "100"},
                        {"PageNumber", "1"}
                    });
    
                var rdsDescribeSlowLogsSuccessResponse =
                    JsonConvert.DeserializeObject<RdsDescribeSlowLogsSuccessResponse>(responseExecute);
    
                if (rdsDescribeSlowLogsSuccessResponse.Engine == null)
                {
                    return ServiceResult.Create(false, ServiceStateEnum.Failed, slowLogRepos);
                }
                //数据库增加Rds慢日志请求
                AddRdsSlowLogRequestRepo(new RdsSlowLogRequestRepo
                {
                    DBInstanceID = dbInstanceId,
                    RequestTime = DateTime.Now,
                    PageNumber = rdsDescribeSlowLogsSuccessResponse.PageNumber,
                    TotalRecordCount = rdsDescribeSlowLogsSuccessResponse.TotalRecordCount,
                    RequestId = rdsDescribeSlowLogsSuccessResponse.RequestId,
                    EndTime = rdsDescribeSlowLogsSuccessResponse.EndTime,
                    StartTime = rdsDescribeSlowLogsSuccessResponse.StartTime,
                    Engine = rdsDescribeSlowLogsSuccessResponse.Engine,
                    PageRecordCount = rdsDescribeSlowLogsSuccessResponse.PageRecordCount
                });
                //保存通过筛选参数请求结果到数据库对象列表
                SaveGetSqlSlowLogByParameterResponseDataToRepoList(response, dbInstanceId,
                    rdsDescribeSlowLogsSuccessResponse,
                    ref slowLogs, ref slowLogRepos);
                //删除通过筛选参数已有数据写入获取的数据到Dms数据库
                DeleteGetSqlSlowLogByParameterExistingWriteSqlSlowLogToDataBase(slowLogRepos, response);    
            }
            return ServiceResult.Create(true, ServiceStateEnum.Success, slowLogRepos);
        }

        /// <summary>
        /// 删除通过筛选参数已有数据写入获取的数据到Dms数据库
        /// </summary>
        /// <param name="slowLogRepos">Mysql慢日志存储对象集合</param>
        /// <param name="response">通过筛选参数获取慢日志请求对象</param>
        private void DeleteGetSqlSlowLogByParameterExistingWriteSqlSlowLogToDataBase(List<SqlSlowLogRepo> slowLogRepos,
            DescriptionGetSqlSlowLogByParameterResponse response)
        {
            DeteteSqlSlowLogByParameter(response);

            AddSqlSlowLogRepoList(slowLogRepos);
        }

        /// <summary>
        /// 删除指定查询日期范围内数据库中SqlSlowLog慢日志的数据
        /// </summary>
        /// <param name="response">通过筛选参数获取慢日志请求对象</param>
        private void DeteteSqlSlowLogByParameter(DescriptionGetSqlSlowLogByParameterResponse response)
        {
            using (DmsDbContext dbContext = new DmsDbContext())
            {
                DateTime startDateTime = DateTime.Parse(response.StarTime).AddDays(-1);
                DateTime endDateTime = DateTime.Parse(response.EndTime);
                var result = dbContext.SqlSlowLog
                    .Where(p => p.Id > 0 && p.AppName.Equals(response.AppName) &&
                                p.CreateTime < endDateTime && p.CreateTime > startDateTime).ToList();
                if (result.Any())
                {
                    foreach (var slowLogRepo in result)
                    {
                        dbContext.SqlSlowLog.Remove(slowLogRepo);
                    }
                    dbContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 保存通过筛选参数请求结果到数据库对象列表
        /// </summary>
        /// <param name="response">通过筛选参数获取慢日志请求对象</param>
        /// <param name="dbInstanceId">数据库实例Id</param>
        /// <param name="rdsDescribeSlowLogsSuccessResponse">Rds慢日志列表成功请求返回对象</param>
        /// <param name="slowLogs">阿里云返回慢日志对象集合</param>
        /// <param name="slowLogRepos">Mysql慢日志存储对象集合</param>
        private void SaveGetSqlSlowLogByParameterResponseDataToRepoList(
            DescriptionGetSqlSlowLogByParameterResponse response, string dbInstanceId,
            RdsDescribeSlowLogsSuccessResponse rdsDescribeSlowLogsSuccessResponse, ref List<SqlSlowLog> slowLogs,
            ref List<SqlSlowLogRepo> slowLogRepos)
        {
            if (slowLogs == null) throw new ArgumentNullException(nameof(slowLogs));

            var slowLoglist = rdsDescribeSlowLogsSuccessResponse.Items.SQLSlowLog;
            foreach (var slowLog in slowLoglist)
            {
                SqlSlowLogRepo sqlSlowLogRepo = new SqlSlowLogRepo
                {
                    DBInstanceId = dbInstanceId,
                    SQLSlowLogRequestId = rdsDescribeSlowLogsSuccessResponse.RequestId,
                    AppName = response.AppName,
                    DBName = slowLog.DBName,
                    SQLText = slowLog.SQLText,
                    MySQLTotalExecutionCounts = slowLog.MySQLTotalExecutionCounts,
                    MySQLTotalExecutionTimes = slowLog.MySQLTotalExecutionTimes,
                    MaxExecutionTime = slowLog.MaxExecutionTime,
                    TotalLockTimes = slowLog.TotalLockTimes,
                    MaxLockTime = slowLog.MaxLockTime,
                    ParseTotalRowCounts = slowLog.ParseTotalRowCounts,
                    ParseMaxRowCount = slowLog.ParseMaxRowCount,
                    ReturnTotalRowCounts = slowLog.ReturnTotalRowCounts,
                    ReturnMaxRowCount = slowLog.ReturnMaxRowCount,
                    CreateTime = slowLog.CreateTime
                };
                slowLogRepos.Add(sqlSlowLogRepo);
            }
            slowLogs.AddRange(slowLoglist);
        }

        /// <summary>
        /// 获取[根据筛选参数获取查询慢日志整合数据]Action请求参数转换后的对象
        /// </summary>
        /// <param name="parameter">筛选参数对象</param>
        /// <returns></returns>
        private DescriptionGetSqlSlowLogByParameterResponse GetSqlSlowLogByParameterRequestUnmarshaller(
            IDictionary<string, object> parameter)
        {
            DescriptionGetSqlSlowLogByParameterRequest request = new DescriptionGetSqlSlowLogByParameterRequest
            {
                AppName = parameter[@"AppName"].ToString(),
                StartTime = parameter[@"StartTime"].ToString(),
                EndTime = parameter[@"EndTime"].ToString()
            };
            UnmarshallerContext unmarshallerContext = new UnmarshallerContext
            {
                ResponseDictionary = request.QueryParameters
            };
            DescriptionGetSqlSlowLogByParameterResponse descriptionGetSqlSlowLogByParameterResponse =
                DescriptionGetSqlSlowLogByParameterUnmarshaller.Unmarshall(unmarshallerContext);
            return descriptionGetSqlSlowLogByParameterResponse;
        }

        #endregion

        #endregion

        public ServiceResult<ServiceStateEnum, List<SqlSlowLogRepo>> GetExportSqlSlowLogResult(
            IDictionary<string, object> parameter)
        {
            return GetSqlSlowLogByParameter(parameter);
        }

        public ServiceResult<ServiceStateEnum, Stream> ExportSqlSlowLogResultToExcel(DataTable dataTable)
        {
            var excelStream = DataImportExcel.DataTableToDefaultStyleExcel(dataTable);
            return ServiceResult.Create(true, ServiceStateEnum.Success, excelStream);
        }

        #region 私有方法

        /// <summary>
        /// 创建Rds请求
        /// </summary>
        /// <returns></returns>
        private static RdsRequest CreateRequest()
        {
            string accessKeyId = ConfigHelper.GetConfigString("ACCESS_KEY_ID");
            string accessKeySecret = ConfigHelper.GetConfigString("ACCESS_KEY_SECRET");
            return new RdsRequest(accessKeyId, accessKeySecret);
        }

        #endregion
    }
}