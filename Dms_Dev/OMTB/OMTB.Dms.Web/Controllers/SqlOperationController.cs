using System;
using System.IO;
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
    /// SQL操作控制器
    /// </summary>
    /// [IpAuthorization]
    public class SqlOperationController : BaseController
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
        // Get: /SqlOperation/GetDbconnectInfoList
        /// <summary>
        /// 获取数据库连接信息集合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDbconnectInfoList()
        {
            var result = DiConfigService.GetDbconnectInfoList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SqlOperation/GetSqlExcuteResult
        /// <summary>
        /// 获取Sql执行返回结果集
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSqlExcuteResult(GetSqlExcuteResultViewModel model)
        {
            if (model?.SqlString == null || model.DbConnectName == null)
            {
                return Json(ServiceResult.Create(false, ServiceStateEnum.ParameterNotNull));
            }

            var dbConnetInfoResult = DiConfigService.GetDbconnectInfoByName(model.DbConnectName);
            if (!dbConnetInfoResult.Success)
            {
                return Json(dbConnetInfoResult);
            }

            SqlOperationService sqlOperationService = new SqlOperationService();
            var excuteResult = sqlOperationService.GetSqlExcuteResult(model.SqlString, dbConnetInfoResult.Data);

            //插入操作日志
            DiLogService.AddLog(new LogRepo
            {
                OperationType = "Sql执行",
                OperationContext = model.SqlString,
                Ip = IpHelper.GetClientIp(),
                OperationResult = "成功",
                OperationDate = DateTime.Now,
                Remark = model.DbConnectName
            });
            JsonResult jsonResult = Json(excuteResult);
            jsonResult.MaxJsonLength = ConfigHelper.GetConfigInt("MaxJsonLength");
            return jsonResult;
        }

        // POST: /SqlOperation/ExportSqlExcuteResultToExcel
        /// <summary>
        /// 导出Sql执行返回结果到Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportSqlExcuteResultToExcel(ExportSqlExcuteResultToExcelViewModel model)
        {
            if (model?.SqlString == null || model.DbConnectName == null)
            {
                return Json(ServiceResult.Create(false, ServiceStateEnum.ParameterError), JsonRequestBehavior.AllowGet);
            }
            var dbConnetInfoResult = DiConfigService.GetDbconnectInfoByName(model.DbConnectName);
            if (!dbConnetInfoResult.Success)
            {
                return Json(ServiceResult.Create(false, dbConnetInfoResult.State), JsonRequestBehavior.AllowGet);
            }
            SqlOperationService sqlOperationService = new SqlOperationService();
            var result = sqlOperationService.GetExportResult(model.SqlString, dbConnetInfoResult.Data);
            if (!result.Success)
            {
                return Json(ServiceResult.Create(false, result.State), JsonRequestBehavior.AllowGet);
            }
            if (result.Data.Rows.Count > 0)
            {
                string filePath = HostingEnvironment.MapPath("~/Files/");
                string fileName = "SqlExcuteResult_" + DateTime.Now.ToString("yyyyMMddHHmmss_") +
                                  RandomCodeHelper.NewRandomCode(5) + ".xls";
                string fileSavePath = filePath + fileName;
                MemoryStream stream = (MemoryStream) sqlOperationService.ExportSqlExcuteResultToExcel(result.Data).Data;
                stream.Seek(0, SeekOrigin.Begin);
                FileStream fileStream = new FileStream(fileSavePath, FileMode.Create);
                stream.Position = 0;
                stream.WriteTo(fileStream);
                fileStream.Close();

                //插入操作日志
                DiLogService.AddLog(new LogRepo
                {
                    OperationType = "导出Sql执行结果",
                    OperationContext = model.SqlString,
                    Ip = IpHelper.GetClientIp(),
                    OperationResult = "成功",
                    OperationDate = DateTime.Now,
                    Remark = model.DbConnectName
                });
                return Json(ServiceResult.Create(true, ServiceStateEnum.Success,
                    new {FileUrl = "/Files/", FileName = fileName}));
            }
            return Json(ServiceResult.Create(false, ServiceStateEnum.NoDataExported), JsonRequestBehavior.AllowGet);
        }
    }
}