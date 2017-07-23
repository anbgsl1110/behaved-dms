using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using Memcached.ClientLibrary;

namespace OMTB.Component.Util
{
    public class MySession : IMySession
    {
        private SockIOPool pool;

        private int sessionHour;

        private string cookieDomain;
        private string preFix = string.Empty;

        public ICache mc;

        /// <summary>
        /// session构造函数
        /// </summary>
        public MySession(ICache _cache)
        {
            NameValueCollection nvc = ConfigurationManager.AppSettings;

            this.preFix = nvc["SessionArea"];
            this.sessionHour = int.Parse(nvc["SessionExpireHours"]);
            this.cookieDomain = nvc["SessionCookieDomain"];
            mc = _cache;
        }

        public void Set<T>(string key, T value)
        {
            key = preFix + key;
            string sessionId = GetSessionId();
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = SetSessionId();
            }

            //TODO 这里高并发时，同一个sessionid的dict可能会被相互覆盖，有时间必须重写加锁
            var cachedItem = mc.Get<Dictionary<string, object>>(sessionId);
            if (cachedItem == null)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic[key] = value;
                mc.Set(sessionId, dic, DateTime.Now.AddHours(sessionHour));
            }
            else
            {
                Dictionary<string, object> dic = cachedItem;
                dic[key] = value;
                mc.Set(sessionId, dic, DateTime.Now.AddHours(sessionHour));
            }
        }

        //简单的超时管理，主要用于session缓存 设置间隔，避免重复set造成不必要的开销
        private ConcurrentDictionary<string, DateTime> keyTimeOut;
        private ConcurrentDictionary<string, DateTime> KeyTimeOut
        {
            get
            {
                if (this.keyTimeOut == null)
                {
                    this.keyTimeOut = new ConcurrentDictionary<string, DateTime>();
                }
                return this.keyTimeOut;
            }
            set
            {
                this.keyTimeOut = value;
            }
        }

        public T Get<T>(string key)
        {
            key = preFix + key;
            string sessionId = GetSessionId();
            T result = default(T);
            if (string.IsNullOrEmpty(sessionId))
            {
                return result; //浏览器cookie["sessionId"]不存在，返回null
            }

            var cachedItem = mc.Get<Dictionary<string, object>>(sessionId);
            if (cachedItem == null)
            {
                return result; //服务器上该session不存在
            }

            Dictionary<string, object> dic = cachedItem as Dictionary<string, object>;

            if (!dic.ContainsKey(key))
            {
                return result;
            }

            // 获取session对象后，延迟对象保存时间
            // duji
            // 20130830
            var timeOut = DateTime.Now.AddMinutes(15);
            //最近是否存储过session对象
            if (KeyTimeOut.Keys.Contains(sessionId))
            {
                //当前时间是否已经超过限制时间
                if (DateTime.Now > KeyTimeOut[sessionId])
                {
                    //超时 
                    //继续延长session时间,修改session保持的时间，保证缓存间隔15分钟以上
                    mc.Set(sessionId, dic, DateTime.Now.AddHours(sessionHour));
                    KeyTimeOut[sessionId] = timeOut;
                }
            }
            else
            {
                //最近未存储过session对象，记录session保存时间，保证缓存间隔15分钟以上
                KeyTimeOut.TryAdd(sessionId, timeOut);
                mc.Set(sessionId, dic, DateTime.Now.AddHours(sessionHour));
            }

            object value = dic[key];

            if (value is T)
            {
                result = (T)value;
            }

            return result;
        }

        public object this[string key]
        {
            get
            {
                return Get<object>(key);
            }
            set
            {
                Set(key, value);
            }
        }

        public void Clear(string sessionId = null)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = GetSessionId();
                //sessionId = null说明是删自己的session，要删除cookie
                DeleteCookie();
            }
            if (!string.IsNullOrEmpty(sessionId))
            {
                mc.Delete(sessionId);
            }
        }

        public string GetSessionId()
        {
            try
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["SessionId"];
                if (cookie != null)
                {
                    //cookie.Expires = DateTime.Now.AddHours(4);
                    return cookie.Value;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #region 内部方法

        /// <summary>
        /// 第一次给session付值时创建
        /// </summary>
        /// <returns></returns>
        private string SetSessionId()
        {
            try
            {
                HttpCookie cookie = new HttpCookie("SessionId");
                if (cookieDomain.ToLower() != "localhost")
                {
                    cookie.Domain = cookieDomain; //当要跨域名访问的时候,给cookie指定域名即可,格式为xxx.com
                }
                //cookie.Expires = DateTime.MaxValue; //关闭浏览器session失效
                cookie.Value = preFix + "_" + Guid.NewGuid();
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                return cookie.Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 退出后清除Cookie保存的sessionId
        /// </summary>      
        /// <returns></returns>
        private bool DeleteCookie()
        {
            try
            {
                HttpCookie cookie = new HttpCookie("SessionId");
                cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                System.Web.HttpContext.Current.Request.Cookies.Remove("SessionId");
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
