using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMTB.Component.Util
{
    public interface ICache
    {
        void Add<T>(string key, T value);
        void Add<T>(string key, T value, DateTime expired);
        void Delete(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value);
        void Set<T>(string key, T value, DateTime expired);
        T Get<T>(string key, Func<T> acquire);
        T Get<T>(string key, Func<T> acquire, DateTime expiry, bool isRefreshForce = false);
    }
}
