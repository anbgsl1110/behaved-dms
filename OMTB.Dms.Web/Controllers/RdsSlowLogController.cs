using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using OMTB.Component.Util;
using OMTB.Dms.Common;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Service;
using OMTB.Dms.Web.Filters;
using OMTB.Dms.Web.Helper;
using OMTB.Dms.Web.Models;

namespace OMTB.Dms.Web.Controllers
{
    /// <summary>
    /// 阿里云Rds慢日志控制器
    /// </summary>
    [IpAuthorization]
    public class RdsSlowLogController : BaseController
    {
        /// <summary>
        /// SQl操作界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        //
        // Get: /RdsSlowLog/GetAppInfoList
        /// <summary>
        /// 获取应用程序信息集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAppInfoList()
        {
            var result = DiConfigService.GetAppInfoList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //Post: /RdsSlowLog/GetRdsSelectResult
        /// <summary>
        /// 获取Rds慢日志查询结果
        /// </summary>
        /// <param name="request">获取Rds慢日志查询结构前端请求对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRdsSelectResult(GetRdsSelectResultRequest request)
        {
            RdsSlowLogService rdsSlowLogService = new RdsSlowLogService();

            var dateRangeEnum = getDateRangeEnum(request.DateRange);
            var result = rdsSlowLogService.GetSqlSlowLogByAppName(request.AppName, (int) dateRangeEnum);
            
            //插入操作日志
            DiLogService.AddLog(new LogRepo
            {
                OperationType = "慢日志查询",
                OperationContext = String.Format(@"AppName:{0},DateRange:{1}",request.AppName,request.DateRange),
                Ip = IpHelper.GetClientIp(),
                OperationResult = "成功",
                OperationDate = DateTime.Now,
                Remark = ""
            });
            
            return Json(result);
        }

        // Post: /RdsSlowLog/ExportSqlSlowLogResultToExcel
        /// <summary>
        /// 导出慢日志查询结果到Excel
        /// </summary>
        /// <param name="request">导出慢日志查询结果到Excel前端请求对象</param>
        /// <returns></returns>
        public ActionResult ExportSqlSlowLogResultToExcel(ExportSqlSlowLogResultToExcelRequest request)
        {
            if (request == null)
            {
                return Json(ServiceResult.Create(false, ServiceStateEnum.ParameterNotNull),
                    JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(request.AppName) || string.IsNullOrWhiteSpace(request.DateRange))
            {
                return Json(ServiceResult.Create(false, ServiceStateEnum.ParameterError), JsonRequestBehavior.AllowGet);
            }
            RdsSlowLogService rdsSlowLogService = new RdsSlowLogService();
            DateRangeEnum dateRangeEnum = getDateRangeEnum(request.DateRange);
            var dateInterval = GetRequestDateInterval(dateRangeEnum);
            IDictionary<string, object> parameter = new Dictionary<string, object>
            {
                {@"AppName", request.AppName},
                {@"StartTime", dateInterval.Item1},
                {@"EndTime", dateInterval.Item2}
            };

            var exportSqlSlowLogResult = rdsSlowLogService.GetExportSqlSlowLogResult(parameter);
            if (!exportSqlSlowLogResult.Success)
            {
                return Json(exportSqlSlowLogResult, JsonRequestBehavior.AllowGet);
            }
            if (exportSqlSlowLogResult.Data.Any())
            {
                var exportData = exportSqlSlowLogResult.Data;
                DataTable dataTable = DataTableHelper.ConvertEnumerableToDataTable(exportData);
                MemoryStream stream = (MemoryStream) rdsSlowLogService.ExportSqlSlowLogResultToExcel(dataTable).Data;
                stream.Seek(0, SeekOrigin.Begin);

                string filePath = HostingEnvironment.MapPath("~/Files/");
                string fileName = "slowlog_" + DateTime.Parse(dateInterval.Item1).ToString("yyyyMMdd") + "_" +
                                  DateTime.Parse(dateInterval.Item2).ToString("yyyyMMdd") + ".xls";
                string fileSavePath = filePath + fileName;
                FileStream fileStream = new FileStream(fileSavePath, FileMode.Create);

                stream.Position = 0;
                stream.WriteTo(fileStream);
                fileStream.Close();
                
                //插入操作日志
                DiLogService.AddLog(new LogRepo
                {
                    OperationType = "导出慢日志",
                    OperationContext = String.Format(@"AppName:{0},DateRange:{1}",request.AppName,request.DateRange),
                    Ip = IpHelper.GetClientIp(),
                    OperationResult = "成功",
                    OperationDate = DateTime.Now,
                    Remark = ""
                });
                return Json(ServiceResult.Create(true, ServiceStateEnum.Success, new
                {
                    FileUrl = "/Files/",
                    FileName = fileName
                }));
            }
            return Json(ServiceResult.Create(false, ServiceStateEnum.NoDataExported), JsonRequestBehavior.AllowGet);
        }

        #region 私有方法

        /// <summary>
        /// 获取日期枚举
        /// </summary>
        /// <param name="dateRange">日期范围</param>
        /// <returns></returns>
        private DateRangeEnum getDateRangeEnum(string dateRange)
        {
            switch (dateRange)
            {
                case @"前一天":
                    return DateRangeEnum.OneDaY;
                case @"前三天":
                    return DateRangeEnum.ThreeDay;
                case @"前七天":
                    return DateRangeEnum.SevenDay;
                default:
                    return DateRangeEnum.OneDaY;
            }
        }

        /// <summary>
        /// 获取请求开始和结束时间
        /// </summary>
        /// <param name="dateRangeEnum">日期范围枚举</param>
        /// <returns></returns>
        private Tuple<string, string> GetRequestDateInterval(DateRangeEnum dateRangeEnum)
        {
            string startTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
            string endTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddZ");
            switch (dateRangeEnum)
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

        #endregion
    }
}