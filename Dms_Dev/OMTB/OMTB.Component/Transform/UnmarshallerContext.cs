using System;
using System.Collections.Generic;
using OMTB.Component.Http;
using OMTB.Component.Util;

namespace OMTB.Component.Transform
{
    public class UnmarshallerContext
    {
        public Dictionary<string, string> ResponseDictionary { get; set; }
        public HttpResponse HttpResponse { get; set; }

        public int? IntegerValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return int.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return null;
        }

        public string StringValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return DictionaryUtil.Get(ResponseDictionary, key);
            }
            return null;
        }

        public long? LongValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return long.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return null;
        }

        public bool? BooleanValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return bool.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return null;
        }

        public float? FloatValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return float.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return null;
        }

        public double? DoubleValue(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return double.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return null;
        }

        public T? EnumValue<T>(string key) where T : struct
        {
            string value = StringValue(key);
            if (null == value)
            {
                return null;
            }
            return (T)Enum.Parse(typeof(T), value);
        }

        public int Length(string key)
        {
            if (null != DictionaryUtil.Get(ResponseDictionary, key))
            {
                return int.Parse(DictionaryUtil.Get(ResponseDictionary, key));
            }
            return 0;
        }
    }
}