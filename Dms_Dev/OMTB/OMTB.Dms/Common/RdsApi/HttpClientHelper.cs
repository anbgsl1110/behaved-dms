using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OMTB.Dms.Common.RdsApi
{
    //通过预热与长连接优化HttpClient请求
    public class HttpClienthelper
    {
        private static readonly HttpClient HttpClient;

        static HttpClienthelper()
        {
            string testUrl = @"http://rds.aliyuncs.com";
            
            HttpClient = new HttpClient() { BaseAddress = new Uri(testUrl) };

            //帮HttpClient热身
            HttpClient.SendAsync(new HttpRequestMessage {
                    Method = new HttpMethod("HEAD"), 
                    RequestUri = new Uri(testUrl + "/") })
                .Result.EnsureSuccessStatusCode();
        }

        public async Task<string> PostAsync(List<KeyValuePair<string, string>> parameters)
        {
            var response = await HttpClient.PostAsync("/", new FormUrlEncodedContent(parameters));

            return await response.Content.ReadAsStringAsync();
        }
    }
}