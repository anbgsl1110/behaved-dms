using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace OMTB.Dms.Common.RdsApi
{
    /// <summary>
    /// 调用阿里云Rds API。
    /// </summary>
    public class RdsRequest
    {
        private const String API_VERSION = "2014-08-15";
        private const String ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        private const String ENCODING = "UTF-8";
        private const String RESPONSE_FORMAT = "JSON";

        // Endpoint请以"/"结尾
        private Uri endpoint = new Uri("http://rds.aliyuncs.com/");
        private String httpMethod = "GET";

        private String accessKeyId;
        private String accessKeySecret;

        public RdsRequest(String accessKeyId, String accessKeySecret)
        {
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
        }

        public String Execute(String action, IDictionary<String, String> parameters)
        {
            if (parameters == null) {
                parameters = new Dictionary<String, String>();
            }

            // 加入公共请求参数
            AddCommonParams(action, parameters);

            // 发送请求
            return SendRequest(parameters);
        }

        private void AddCommonParams(String action, IDictionary<String, String> parameters)
        {
            parameters.Add("Action", action);
            parameters.Add("Version", API_VERSION);
            parameters.Add("AccessKeyId", accessKeyId);
            parameters.Add("TimeStamp", FormatIso8601Date(DateTime.Now));
            parameters.Add("SignatureMethod", "HMAC-SHA1");
            parameters.Add("SignatureVersion", "1.0");
            parameters.Add("SignatureNonce", Guid.NewGuid().ToString()); // 可以使用GUID作为SignatureNonce
            parameters.Add("Format", RESPONSE_FORMAT);

            // 计算签名，并将签名结果加入请求参数中
            parameters.Add("Signature", ComputeSignature(parameters));
        }

        private String ComputeSignature(IDictionary<String, String> parameters)
        {

            const String SEPARATOR = "&";

            // 生成规范化请求字符串
            StringBuilder canonicalizedQueryString = new StringBuilder();

            var orderedParameters = parameters.OrderBy(e => e.Key);
            foreach(var p in orderedParameters) {
                canonicalizedQueryString.Append("&")
                    .Append(PercentEncode(p.Key)).Append("=")
                    .Append(PercentEncode(p.Value));
            }

            // 生成用于计算签名的字符串 stringToSign
            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append(httpMethod).Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/")).Append(SEPARATOR);

            stringToSign.Append(PercentEncode(
                canonicalizedQueryString.ToString().Substring(1)));

            // 注意accessKeySecret后面要加入一个字符"&"
            String signature = CalculateSignature(accessKeySecret + "&",
                                                  stringToSign.ToString());
            return signature;
        }

        private String SendRequest(IDictionary<String, String> parameters) {
            String query = ParamsToQueryString(parameters);
            Uri url = new Uri(endpoint.ToString() + "?" + query);

            var request = WebRequest.Create(url);
            try
            {
                var response = request.GetResponse() as HttpWebResponse;

                return ReadResponse(response);
            } catch (WebException e)
            {
                if (e.Response != null)
                {
                    return ReadResponse(e.Response);
                }
                throw e;
            }
        }

        private static String ReadResponse(WebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream))
                {
                    var body = reader.ReadToEnd();
                    return body;
                }
            }
        }

        private static String FormatIso8601Date(DateTime date)
        {
            // 注意使用UTC时间
            return date.ToUniversalTime().ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));
        }

        private static String CalculateSignature(String key, String stringToSign)
        {
            // 使用HmacSHA1算法计算HMAC值
            using (var algorithm = KeyedHashAlgorithm.Create("HMACSHA1"))
            {
                algorithm.Key = Encoding.GetEncoding(ENCODING).GetBytes(key.ToCharArray());
                return Convert.ToBase64String(
                    algorithm.ComputeHash(
                        Encoding.GetEncoding(ENCODING).GetBytes(stringToSign.ToCharArray())));
            }
        }

        private static string PercentEncode(String value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding(ENCODING).GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        private static String ParamsToQueryString(IDictionary<String, String> parameters)
        {
            if (parameters == null || parameters.Count == 0){
                return null;
            }

            StringBuilder paramString = new StringBuilder();
            bool first = true;
            foreach(var p in parameters){
                String key = p.Key;
                String val = p.Value;

                if (!first){
                    paramString.Append("&");
                }

                paramString.Append(PercentEncode(key));

                if (val != null){
                    paramString.Append("=").Append(PercentEncode(val));
                }

                first = false;
            }

            return paramString.ToString();
        }
    }
}
