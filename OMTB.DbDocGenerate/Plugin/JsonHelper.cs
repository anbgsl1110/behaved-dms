using Newtonsoft.Json;

namespace DbDocGenerate.Plugin
{
    public class JsonHelper
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
        };

        public static string SerializeObject(object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public static object DeserializeObject(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(value, settings);
        }

        public static T DeserializeObject<T>(string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, settings);
        }

    }
}