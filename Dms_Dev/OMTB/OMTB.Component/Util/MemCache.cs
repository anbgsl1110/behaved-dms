using System;
using Memcached.ClientLibrary;

namespace OMTB.Component.Util
{
    public class MemCache : ICache
    {
        private SockIOPool pool;

        MemcachedClient mc = new MemcachedClient();
        private string preFix = string.Empty;

        public MemCache(string[] strServer, string pre)
        {
            preFix = pre;
            try
            {
                pool = SockIOPool.GetInstance();
                pool.SetServers(strServer);

                //初始化连接数
                pool.InitConnections = 3;
                //最小连接数
                pool.MinConnections = 3;
                //最大连接数
                pool.MaxConnections = 5;


                pool.SocketConnectTimeout = 1000;
                pool.SocketTimeout = 3000;

                pool.MaintenanceSleep = 30;
                pool.Failover = true;

                pool.Nagle = false;
                pool.Initialize();
            }
            catch (Exception ex)
            {
                //TODO 记录日志
                throw ex;
            }
        }

        public void Delete(string key)
        {
            key = preFix + key;
            mc.Delete(key);
        }

        public void Add<T>(string key, T value, DateTime date)
        {
            key = preFix + key;
            if (value == null)
            {
                mc.Delete(key);
            }
            else
            {
                mc.Add(key, value, date);
            }
        }

        public void Add<T>(string key, T value)
        {
            key = preFix + key;
            if (value == null)
            {
                mc.Delete(key);
            }
            else
            {
                mc.Add(key, value, DateTime.Now.AddMinutes(30));
            }
        }

        public void Set<T>(string key, T value)
        {
            key = preFix + key;
            _Set<T>(key, value);
        }

        public void Set<T>(string key, T value, DateTime date)
        {
            key = preFix + key;
            _Set(key, value, date);
        }

        public T Get<T>(string key, Func<T> acquire)
        {
            key = preFix + key;
            if (mc.KeyExists(key))
            {
                return _Get<T>(key);
            }
            else
            {
                T result = acquire();
                _Set(key, result);
                return result;
            }
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
                if (mc.KeyExists(key))
                {
                    return _Get<T>(key);
                }
                else
                {
                    T result = acquire();
                    _Set(key, result, expiry);
                    return result;
                }
            }
        }

        public T Get<T>(string key)
        {
            key = preFix + key;
            return _Get<T>(key);
        }

        public object this[string key]
        {
            get
            {
                key = preFix + key;
                return _Get<object>(key);
            }
            set
            {
                key = preFix + key;
                _Set(key, value);
            }
        }

        #region 私有 不对 key处理的函数
        private T _Get<T>(string key)
        {
            T result = default(T);
            object value = mc.Get(key);

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
                mc.Delete(key);
            }
            else
            {
                mc.Set(key, value);
            }
        }

        private void _Set<T>(string key, T value, DateTime date)
        {
            if (value == null)
            {
                mc.Delete(key);
            }
            else
            {
                mc.Set(key, value, date);
            }
        }
        #endregion
    }
}
