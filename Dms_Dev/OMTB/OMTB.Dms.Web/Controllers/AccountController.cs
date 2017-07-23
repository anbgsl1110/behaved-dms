using System;
using System.Web.Mvc;
using OMTB.Dms.Common;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Service;
using OMTB.Dms.Web.Helper;
using OMTB.Dms.Web.Models;
using SchoolPal.Cloud.Services.Sms;
using SchoolPal.Toolkit;

namespace OMTB.Dms.Web.Controllers
{
    /// <summary>
    /// 账户控制器
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login
        /// <summary>
        /// 登录界面
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            //获取默认获取验证码手机号
            ConfigService configService = new ConfigService();
            var result = configService.GetDefaultValidatePhone();
            
            ViewBag.Phone = result.Data;
            return View();
        }

        //
        // POST: /Account/Login
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SignIn(LoginViewModel model)
        {
            //前端参数验证
            if (model.Code  == null)
            {
                return Json(ServiceStateEnum.ValidateCodeNotNull);
            }
            // 验证码发送验证
            if (Session["DmsLoginPhone"] == null || Session["DmsLoginSmsCodeId"] == null)
            {
                return Json(ServiceStateEnum.ValidateCodeError);
            }
            // 验证登录信息
            using (var smsService = new SmsServiceClient())
            {
                var result = smsService.CheckValidateCode((long) Session["DmsLoginSmsCodeId"],
                    Session["DmsLoginPhone"].ToString(), model.Code);
                if (result == ValidateState.Success)
                {
                    //设置登录用户Ip信息
                    string ip = IpHelper.GetClientIp();
                    Session["CU-Ip"] = ip;
                    
                    //插入操作日志
                    DiLogService.AddLog(new LogRepo
                    {
                        OperationType = "登录",
                        OperationContext = "",
                        Ip = ip,
                        OperationResult = "成功",
                        OperationDate = DateTime.Now,
                        Remark = ""
                    });
                    return Json(ServiceStateEnum.Success);
                }
                return Json(ServiceStateEnum.ValidateCodeError);
            }
        }

        //
        // POST: /Account/SendCode
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendCode(string phone)
        {
            // 发送验证码
            using (var smsService = new SmsServiceClient())
            {
                string smsCode = ValidateCodeGenerator.CreateValidateCode(6);
                var result = smsService.SendValidateCode("1CourseCrm_WebSite", phone, "1CourseCrm_ValidateCode",
                    smsCode, "数据管理Dms登录", 60 * 30);
                if (result.State == SendState.Success)
                {
                    //验证码短信发送成功
                    Session["DmsLoginPhone"] = phone;
                    Session["DmsLoginSmsCodeId"] = result.Data;
                    return Json(ServiceStateEnum.Success);
                }
                //验证码短信发送失败          
                return Json(ServiceStateEnum.Failed);
            }
        }
    }
}