using System;
using System.Web.Mvc;
using OMTB.Dms.Service;

namespace OMTB.Dms.Web.Filters
{
    /// <summary>
    /// 授权筛选器
    /// </summary>
    public class IpAuthorization : FilterAttribute,IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            ConfigService configService = new ConfigService();
            var ipWhiteInfo = configService.GetIpWhiteInfo();
            if (ipWhiteInfo.Success && ipWhiteInfo.Data.IpWhiteAddressList.Count > 0)
            {
                var ipWhiteAddressList = ipWhiteInfo.Data.IpWhiteAddressList;
                if (filterContext.HttpContext.Session == null)
                {
                    filterContext.Result = new RedirectResult("/Base/Nofunc");               
                }
                else
                {
                    string curentUsrIp  = Convert.ToString(filterContext.HttpContext.Session["CU-Ip"]);
                    //Ip地址鉴权
                    if (!ipWhiteAddressList.Contains(curentUsrIp))
                    {
                        filterContext.Result = new RedirectResult("/Base/Nofunc");
                    }
                }
            }
        }
    }
}