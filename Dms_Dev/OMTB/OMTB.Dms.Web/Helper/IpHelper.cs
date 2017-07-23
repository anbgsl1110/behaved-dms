using System;

namespace OMTB.Dms.Web.Helper
{
    public static class IpHelper
    {
        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取客户端Ip地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIp()
        {
            string userIP = "未获取用户IP";

            try
            {
                if (System.Web.HttpContext.Current == null)
                    return "";

                //CDN加速后取到的IP
                var customerIp = System.Web.HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (!string.IsNullOrEmpty(customerIp))
                {
                    //判断获取是否成功，并检查IP地址的格式
                    if (!string.IsNullOrEmpty(customerIp) && IsIp(customerIp))
                    {
                        return customerIp;
                    }
                }

                customerIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];


                if (!String.IsNullOrEmpty(customerIp))
                {
                    if (!string.IsNullOrEmpty(customerIp) && IsIp(customerIp))
                    {
                        return customerIp;
                    }
                }

                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    //如果客户端使用了代理服务器，则利用HTTP_X_FORWARDED_FOR找到客户端IP地址
                    customerIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (customerIp == null)
                        //否则直接读取REMOTE_ADDR获取客户端IP地址
                        customerIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                else
                {
                    customerIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                //前两者均失败，则利用Request.UserHostAddress属性获取IP地址，但此时无法确定该IP是客户端IP还是代理IP
                if (String.Compare(customerIp, "unknown", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return System.Web.HttpContext.Current.Request.UserHostAddress;

                }
                return customerIp;
            }
            catch
            {
                // ignored
            }

            return userIP;
        }
    }
}