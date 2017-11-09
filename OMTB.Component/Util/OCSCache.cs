using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System.Net;

namespace OMTB.Component.Util
{
    public class OCSCache : ICache
    {
        MemcachedClient MemClient = MemCached.getInstance();

        public OCSCache(string pre)
        {
            preFix = pre;
        }

        private string preFix = string.Empty;
        public void Delete(string key)
        {
            key = preFix + key;
            MemClient.Remove(key);
        }

        public void Add<T>(string key, T value)
        {
            key = preFix + key;
            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Add, key, value, DateTime.Now.AddMinutes(30));
            }
        }

        public void Add<T>(string key, T value, DateTime expired)
        {
            key = preFix + key;
            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Add, key, value, expired);
            }
        }

        public void Update<T>(string key, T value, DateTime expired)
        {
            key = preFix + key;
            _Update<T>(key, value, expired);
        }

        public void Update<T>(string key, T value)
        {

            key = preFix + key;
            _Update<T>(key, value);
        }

        public void Set<T>(string key, T value, DateTime expired)
        {
            key = preFix + key;
            _Set<T>(key, value, expired);
        }

        public void Set<T>(string key, T value)
        {

            key = preFix + key;
            _Set<T>(key, value);
        }


        public T Get<T>(string key)
        {
            key = preFix + key;
            return _Get<T>(key);
        }


        public T Get<T>(string key, Func<T> acquire)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key, Func<T> acquire, DateTime expiry, bool isRefreshForce = false)
        {
            key = preFix + key;
            if (isRefreshForce)
            {
                T result = acquire();
                _Set(key, result, expiry);
                return result;
            }
            else
            {
                object res;
                if (MemClient.TryGet(key, out res))
                {
                    return (res is T) ? (T)res : default(T);
                }
                else
                {
                    T result = acquire();
                    _Set(key, result, expiry);
                    return result;
                }
            }
        }

        #region 私有 不对 key处理的函数

        private T _Get<T>(string key)
        {
            T result = default(T);
            object value = MemClient.Get(key);

            if (value is T)
            {
                result = (T)value;
            }

            return result;
        }

        private void _Set<T>(string key, T value)
        {
            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Set, key, value);
            }

        }

        private void _Set<T>(string key, T value, DateTime expired)
        {
            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Set, key, value, expired);
            }

        }


        private void _Update<T>(string key, T value)
        {

            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Replace, key, value, DateTime.Now.AddMinutes(30));
            }
        }

        private void _Update<T>(string key, T value, DateTime expired)
        {
            if (value == null)
            {
                MemClient.Remove(key);
            }
            else
            {
                MemClient.Store(StoreMode.Replace, key, value, expired);
            }
        }
        #endregion

    }
    public sealed class MemCached
    {
        private static MemcachedClient MemClient;
        static readonly object padlock = new object();
        //线程安全的单例模式
        public static MemcachedClient getInstance()
        {
            if (MemClient == null)
            {
                lock (padlock)
                {
                    if (MemClient == null)
                    {
                        MemClientInit();
                    }
                }
            }
            return MemClient;
        }

        static void MemClientInit()
        {
            try
            {
                //初始化缓存
                MemcachedClientConfiguration memConfig = new MemcachedClientConfiguration();
                //your_instanceid.m.cnhzalicm10pub001.ocs.aliyuncs.com
                //42.121.0.214

                //OCSPort
                //OCSAddress
                var ocsAddress = System.Configuration.ConfigurationManager.AppSettings["OCSAddress"];
                var ocsPort = System.Configuration.ConfigurationManager.AppSettings["OCSPort"];
                var ocsUserName = System.Configuration.ConfigurationManager.AppSettings["OCSUserName"];
                var ocsPassWord = System.Configuration.ConfigurationManager.AppSettings["OCSPassWord"];
                var ocsMaxPoolSize = System.Configuration.ConfigurationManager.AppSettings["OCSMaxPoolSize"];

                IPAddress newAddress;
                if (ocsAddress.Contains("aliyuncs.com"))
                {
                    newAddress = IPAddress.Parse(Dns.GetHostEntry(ocsAddress).AddressList[0].ToString());
                    //your_instanceid替换为你的OCS实例的ID
                }
                else
                {
                    newAddress = IPAddress.Parse(ocsAddress);
                    //your_instanceid替换为你的OCS实例的ID
                }

                IPEndPoint ipEndPoint = new IPEndPoint(newAddress, Int32.Parse(ocsPort));

                // 配置文件 - ip
                memConfig.Servers.Add(ipEndPoint);
                // 配置文件 - 协议
                memConfig.Protocol = MemcachedProtocol.Binary;
                // 配置文件-权限
                memConfig.Authentication.Type = typeof(PlainTextAuthenticator);
                memConfig.Authentication.Parameters["zone"] = "";
                //ocs 实例Id
                memConfig.Authentication.Parameters["userName"] = ocsUserName;
                //ocs 密码
                memConfig.Authentication.Parameters["password"] = ocsPassWord;
                //下面请根据实例的最大连接数进行设置
                memConfig.SocketPool.MinPoolSize = 5;
                memConfig.SocketPool.MaxPoolSize = ocsMaxPoolSize.ToInt();
                MemClient = new MemcachedClient(memConfig);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
