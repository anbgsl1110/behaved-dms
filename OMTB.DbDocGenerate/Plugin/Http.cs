using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace DbDocGenerate.Plugin
{
    public class Http
    {
        public CookieContainer CC { get; set; }

        public Encoding PageEncode { get; set; }
        public string Absurl { get; set; }
        public string Referer { get; set; }
        public Http()
        {
            CC = new CookieContainer();
            PageEncode = Encoding.UTF8;
        }

        public string IsNotNullHtml(string url)
        {
            for (int i = 0; i < 3; i++)
            {
                var html = this.GetHtml(url);
                if (!string.IsNullOrEmpty(html))
                    return html;
            }
            return "-1";
        }

        public static bool IsUrlEnabled(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                WebResponse response = request.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetHtml(string url)
        {
            HttpWebRequest Myrequest = (HttpWebRequest)WebRequest.Create(Absurl + url);
            Myrequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";

            Myrequest.Headers.Add("Accept-Language", "zh-cn");
            Myrequest.Headers.Add("Accept-Encoding", "gzip, deflate");

            string cookieStr = "";
            CookieCollection cc = CC.GetCookies(new Uri(Absurl));

            for (int i = 0; i < cc.Count; i++)
            {
                cookieStr += cc[i].Name + "=" + cc[i].Value + ";";
            }
            LogManager.WriteLog(LogFile.Trace, cookieStr);
            LogManager.WriteLog(LogFile.Trace, CC.GetCookieHeader(new Uri(Absurl)));
            // Myrequest.Headers.Add("Cookie",cookieStr + "xie=wlf");

            // CC.Add(new Uri(Absurl), new Cookie("xie", "xie", "/"));
            Myrequest.CookieContainer = CC;

            string sHtml = "";
            try
            {
                HttpWebResponse Myresponse = (HttpWebResponse)Myrequest.GetResponse();
                Myresponse.Cookies = CC.GetCookies(Myrequest.RequestUri);
                CC.Add(Myresponse.Cookies);


                // begin-gzip
                string sContentEncoding = Myresponse.GetResponseHeader("Content-Encoding");

                if (sContentEncoding == "gzip")
                {
                    // ToolClass.LogMessage("gzip ok", page);
                    Stream ms = Myresponse.GetResponseStream();
                    MemoryStream msTemp = new MemoryStream();

                    int count = 0;
                    GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
                    byte[] buf = new byte[1000];
                    while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                    { msTemp.Write(buf, 0, count); }
                    sHtml = PageEncode.GetString(msTemp.ToArray());
                    msTemp.Close();
                }
                // end-gzip
                else
                {
                    Stream ms = Myresponse.GetResponseStream();
                    MemoryStream msTemp = new MemoryStream();

                    int count = 0;
                    byte[] buf = new byte[1000];
                    while ((count = ms.Read(buf, 0, buf.Length)) > 0)
                    { msTemp.Write(buf, 0, count); }
                    sHtml = PageEncode.GetString(msTemp.ToArray());

                    //Stream Mystream = Myresponse.GetResponseStream();
                    //sHtml = new StreamReader(Mystream, PageEncode).ReadToEnd();
                    if (PageEncode != Encoding.GetEncoding("GB2312") && sHtml.IndexOf("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\">") != -1)
                    {
                        sHtml = Encoding.GetEncoding("GB2312").GetString(msTemp.ToArray());
                    }
                    msTemp.Close();
                }
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogFile.Warning, url + " " + e.Message);
            }

            if (sHtml.Length > 0)
            {
                LogManager.WriteLog(LogFile.Html, url + "\r\n" + sHtml);
            }

            return sHtml;
        }

        public string GetHtml(Dictionary<string, string> postData, string url, string method)
        {
            string formData = "";
            foreach (string item in postData.Keys)
            {
                formData += item + "=" + postData[item] + "&";
            }
            formData = formData.TrimEnd('&');

            byte[] data = PageEncode.GetBytes(formData);
            // byte[] data = Encoding.ASCII.GetBytes(formData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Absurl + url);
            request.Method = method;    //数据提交方式

            //string cookieStr = "";
            //CookieCollection cc = CC.GetCookies(new Uri(Absurl));
            //for (int i = 0; i < cc.Count; i++)
            //{
            //    cookieStr += cc[i].Name + "=" + cc[i].Value + ";";
            //}
            //request.Headers.Add("Cookie", cookieStr);
            //request.Headers.Add("Accept-Language", "zh-cn");
            //request.Headers.Add("Accept-Encoding", "gzip, deflate");

            if (method == "GET")
            {
                request = (HttpWebRequest)WebRequest.Create(Absurl + url + "?" + formData);
            }

            if (method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/xaml+xml, application/x-ms-xbap, application/x-ms-application, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, application/vnd.ms-xpsdocument, */*";
                request.KeepAlive = true;
            }
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";

            request.CookieContainer = CC;
            if (string.IsNullOrEmpty(Referer))
            { request.Referer = Absurl + url; }
            else { request.Referer = Referer; }

            if (method == "POST")
            {
                //模拟一个UserAgent
                Stream newStream = request.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }

            string WebContent = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = CC.GetCookies(request.RequestUri);
                CC.Add(response.Cookies);

                // begin-gzip
                string sContentEncoding = response.GetResponseHeader("Content-Encoding");

                if (sContentEncoding == "gzip")
                {
                    // ToolClass.LogMessage("gzip ok", page);
                    Stream ms = response.GetResponseStream();
                    MemoryStream msTemp = new MemoryStream();

                    int count = 0;
                    GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);
                    byte[] buf = new byte[1000];
                    while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                    { msTemp.Write(buf, 0, count); }
                    WebContent = PageEncode.GetString(msTemp.ToArray());
                }
                // end-gzip
                else
                {
                    Stream stream = response.GetResponseStream();
                    WebContent = new StreamReader(stream, PageEncode).ReadToEnd();

                    if (PageEncode != Encoding.GetEncoding("gb2312") && WebContent.IndexOf("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\">") != -1)
                    {
                        WebContent = new StreamReader(stream, Encoding.GetEncoding("gb2312")).ReadToEnd();
                    }

                    for (int i = 0; i < response.Cookies.Count; i++)
                    {
                        LogManager.WriteLog(LogFile.Trace, response.Cookies[i].Value);
                    }

                    stream.Close();
                }
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogFile.Warning, Absurl + url + "\t" + formData + " " + e.Message);
            }

            if (!string.IsNullOrEmpty(WebContent))
            {
                LogManager.WriteLog(LogFile.Html, Absurl + url + "\t" + formData + "\r\n" + WebContent);
            }

            return WebContent;
        }


        /// <summary>
        /// 登录网站并获取Cookies
        /// </summary>
        /// <returns>成功登录的Cookie信息</returns>
        public string GetLogin(Dictionary<string, string> postData, string loginUrl)
        {
            /*
            string FormData = "";
            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("__VIEWSTATE", "/wEPDwULLTE0NTM5ODY2NDZkGAEFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYBBSFjdGwwMCRDb250ZW50UGxhY2VIb2xkZXIxJGJfbG9naW5i02ipDJw54JBm2Ix13iqk8sG00A==");
            postData.Add("ctl00$ContentPlaceHolder1$b_bl", "0");
            postData.Add("ctl00$ContentPlaceHolder1$b_login.x", "0");
            postData.Add("ctl00$ContentPlaceHolder1$b_login.y", "0");
            postData.Add("ctl00$ContentPlaceHolder1$b_password", "g.com.cn");
            postData.Add("ctl00$ContentPlaceHolder1$b_username", "XIE");
            postData.Add("ctl00$ContentPlaceHolder1$b_VerifyCode", textBox3.Text.Trim());
            */

            string formData = "";
            foreach (string item in postData.Keys)
            {
                formData += item + "=" + postData[item] + "&";
            }
            formData = formData.TrimEnd('&');


            byte[] data = PageEncode.GetBytes(formData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Absurl + loginUrl);
            request.Method = "POST";    //数据提交方式
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";

            string cookieStr = "";
            CookieCollection cc = CC.GetCookies(new Uri(Absurl));
            for (int i = 0; i < cc.Count; i++)
            {
                cookieStr += cc[i].Name + "=" + cc[i].Value + ";";
            }
            request.Headers.Add("Cookie", cookieStr);

            request.CookieContainer = CC;
            request.Referer = Absurl + loginUrl;

            //模拟一个UserAgent
            Stream newStream = request.GetRequestStream();
            newStream.Write(data, 0, data.Length);

            newStream.Close();

            // ServicePointManager.Expect100Continue = false;
            string WebContent = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = CC.GetCookies(request.RequestUri);

                CC.Add(response.Cookies);

                Stream stream = response.GetResponseStream();
                WebContent = new StreamReader(stream, PageEncode).ReadToEnd();
                stream.Close();
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogFile.Warning, formData + " " + e.Message);
            }

            if (WebContent.Length > 0)
                LogManager.WriteLog(LogFile.Html, formData + "\r\n" + WebContent);

            return WebContent;
        }

        //public Image GetImage(string verifyUrl)
        //{
        //    HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(Absurl + verifyUrl);
        //    // webreq.Referer = "http://localhost:97/login.asp";
        //    webreq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";
        //    webreq.CookieContainer = CC;

        //    Image image1 = null;
        //    try
        //    {
        //        HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
        //        Stream stream = webres.GetResponseStream();
        //        CC.Add(webres.Cookies);

        //        // pictureBox1.Image = Image.FromStream(stream);
        //        image1 = Image.FromStream(stream);
        //        stream.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        LogManager.WriteLog(LogFile.Warning, e.Message);
        //    }
        //    return image1;
        //}
        public Stream GetImageStream(string verifyUrl)
        {
            HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(Absurl + verifyUrl);
            // webreq.Referer = "http://localhost:97/login.asp";
            webreq.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322)";
            webreq.CookieContainer = CC;

            Stream stream = null;
            try
            {
                HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

                stream = webres.GetResponseStream();
                CC.Add(webres.Cookies);
            }
            catch (Exception e)
            {
                LogManager.WriteLog(LogFile.Warning, e.Message);
            }

            // pictureBox1.Image = Image.FromStream(stream);
            // Image image1 = Image.FromStream(stream);

            // stream.Close();
            return stream;
        }
    }
}
