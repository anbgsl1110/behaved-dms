using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Greedy.Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OMTB.Component.Reader;
using OMTB.Component.Util;
using OMTB.Dms.Common;
using OMTB.Dms.Common.RdsApi;
using OMTB.Dms.Data.Database;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;
using OMTB.Dms.Service;
using Xunit;

namespace OMTB.Dms.Test.ServiceTest
{    
    /// <summary>
    /// Rds慢日志服务单元测试
    /// </summary>
    public class RdsSlowLogServiceTest
    {
        private readonly RdsSlowLogService _service = new RdsSlowLogService();

        /// <summary>
        /// 增加Rds慢日志请求单元测试
        /// </summary>
        [Fact]
        public void AddRdsSlowLogRequestRepoTest()
        {
            RdsSlowLogRequestRepo rdsSlowLogRequestRepo = new RdsSlowLogRequestRepo
            {
                RequestId = @"增加Rds慢日志请求单元测试请求Id",
                DBInstanceID = @"单元测试实例名",
                Engine = "Mysql",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(1),
                TotalRecordCount = 20,
                PageNumber = 2,
                PageRecordCount = 4
            };
            _service.AddRdsSlowLogRequestRepo(rdsSlowLogRequestRepo);

            using (var connection = DbFactory.DmsConnection())
            {
                //读取脚本文件内容
                SqlReader reader = (SqlReader) ReaderFactory.CreateInstance(ReaderType.Sql);
                string sqlScriptName1 = "AddRdsSlowLogRequestRepoTest";
                DirectoryInfo appBaseDirectoryInfo = Directory.GetParent(System.AppDomain.CurrentDomain.BaseDirectory);
                if (appBaseDirectoryInfo.Parent?.Parent != null)
                {
                    string appBaseDirectoryString  = appBaseDirectoryInfo.Parent.FullName + "\\SQLScripts\\";
                    string sqlScriptpath1 = appBaseDirectoryString + sqlScriptName1 + ".sql";
                    reader.Read(sqlScriptName1,sqlScriptpath1);

                    var sql = reader.SqlScriptContext[sqlScriptName1];
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        var result = connection.Query(sql);
                        Assert.True(result.Any());
                    }
                    else
                    {
                        Assert.False(true);
                    }
                }
                else
                {
                    Assert.False(true);
                }
            }
        }
        
        /// <summary>
        /// 数据库增加慢日志单元测试
        /// </summary>
        [Fact]
        public void AddSqlSlowLogRepoTest()
        {
            SqlSlowLogRepo sqlSlowLogRepo = new SqlSlowLogRepo
            {
                AppName = "xiaobao",
                DBName = "单元测试",
                SQLText = "数据库增加慢日志单元测试",
                MySQLTotalExecutionCounts = 1,
                MySQLTotalExecutionTimes = 1,
                MaxExecutionTime = 1,
                TotalLockTimes = 1,
                MaxLockTime = 1,
                ParseTotalRowCounts = 1,
                ParseMaxRowCount = 1,
                ReturnTotalRowCounts = 1,
                ReturnMaxRowCount = 1,
                CreateTime = DateTime.Now
            };
            _service.AddSqlSlowLogRepo(sqlSlowLogRepo);
            
            DmsDbContext dmsDbContext = new DmsDbContext();
            using (dmsDbContext)
            {
                var result = dmsDbContext.SqlSlowLog.Count();
                Assert.True(result > 0);
            }
        }

        /// <summary>
        /// 按慢日志集合批量增加慢日志单元测试
        /// </summary>
        [Fact]
        public void AddSqlSlowLogRepoListTest()
        {           
            List<SqlSlowLogRepo> slowLogRepos = new List<SqlSlowLogRepo>();
            for (int i = 0; i < 9; i++)
            {
                SqlSlowLogRepo sqlSlowLogRepo = new SqlSlowLogRepo
                {
                    AppName = "xiaobao",
                    DBName = "单元测试",
                    SQLText = "按慢日志集合批量增加慢日志单元测试",
                    MySQLTotalExecutionCounts = 1,
                    MySQLTotalExecutionTimes = 1,
                    MaxExecutionTime = 1,
                    TotalLockTimes = 1,
                    MaxLockTime = 1,
                    ParseTotalRowCounts = 1,
                    ParseMaxRowCount = 1,
                    ReturnTotalRowCounts = 1,
                    ReturnMaxRowCount = 1,
                    CreateTime = DateTime.Now
                };
                slowLogRepos.Add(sqlSlowLogRepo);
            }
            
            _service.AddSqlSlowLogRepoList(slowLogRepos);
            
            DmsDbContext dmsDbContext = new DmsDbContext();
            using (dmsDbContext)
            {
                var result = dmsDbContext.SqlSlowLog.Count();
                Assert.True(result > 8);
            }
        }

        /// <summary>
        /// 通过Post阿里云Rds慢日志Api测试
        /// </summary>
        [Fact]
        public void AliyunRdsSlowLogApiPostRequestTest()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("user-agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/html");

            //Post请求测试
            var url = @"http://rds.aliyuncs.com/DescribeSlowLogs/";
            
            RdsDescribeSlowLogsRequestParameter data = new RdsDescribeSlowLogsRequestParameter
            {
                Format = "JSON",
                Version = "2014-08-15",
                AccessKeyId = "TMP.AQH86KnrYf6hOvDtbcOR2svc8F1zvxwJ5-b-97c1jT5oWvmY-o7Qe4cebJz5MC4CFQDxS_FvIHVBUvpgPGyLHVZiizd9owIVAOjqIBRGhx8_Dv-oDVQ0K5I1mhe5",
                Signature = "1111",
                SignatureMethod = "HMAC-SHA1",
                Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SignatureVersion = "1.0",
                SignatureNonce = RandomCodeHelper.NewRandomCode(4),                
                //Action = "DescribeSlowLogs",
                DBInstanceId = "rm-wz96sojqf5ffnmg5q",
                StartTime = "2017-07-01Z",
                EndTime = "2017-07-05Z",
                DBName = "dms_dev",
                SortKey = "TotalQueryTimes",
                PageSize = 100,
                PageNumber = 1
            };
            var s = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var jsonString = JsonConvert.SerializeObject(data,s);
            
            HttpContent httpContent =  new StringContent(jsonString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = httpClient.PostAsync(new Uri(url),httpContent).Result;
            var result = response.Content.ReadAsStringAsync().Result;          
            
            #region 通过FormUrlEncodedContent进行参数传递

            /*#region 公共请求参数
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            
            paramList.Add(new KeyValuePair<string, string>("Format", "JSON"));
            paramList.Add(new KeyValuePair<string, string>("Version", "2014-08-15"));
            paramList.Add(new KeyValuePair<string, string>("AccessKeyId", "TMP.AQH86KnrYf6hOvDtbcOR2svc8F1zvxwJ5-b-97c1jT5oWvmY-o7Qe4cebJz5MC4CFQDxS_FvIHVBUvpgPGyLHVZiizd9owIVAOjqIBRGhx8_Dv-oDVQ0K5I1mhe5"));
            paramList.Add(new KeyValuePair<string, string>("Signature", "67567576"));
            paramList.Add(new KeyValuePair<string, string>("SignatureMethod", "HMAC-SHA1"));
            paramList.Add(new KeyValuePair<string, string>("Timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            paramList.Add(new KeyValuePair<string, string>("SignatureVersion", "1.0"));
            paramList.Add(new KeyValuePair<string, string>("SignatureNonce", RandomCodeHelper.NewRandomCode(4)));
            
            #endregion
            
            #region 慢日志请求参数
            
            paramList.Add(new KeyValuePair<string, string>("Action", "DescribeSlowLogs"));
            paramList.Add(new KeyValuePair<string, string>("DBInstanceId", "rm-wz96sojqf5ffnmg5q"));
            paramList.Add(new KeyValuePair<string, string>("StartTime", "2017-07-01Z"));
            paramList.Add(new KeyValuePair<string, string>("EndTime", "2017-07-05Z"));
            paramList.Add(new KeyValuePair<string, string>("DBName", "dms_dev"));
            paramList.Add(new KeyValuePair<string, string>("SortKey", "TotalQueryTimes"));
            paramList.Add(new KeyValuePair<string, string>("PageSize", "100"));
            paramList.Add(new KeyValuePair<string, string>("PageNumber", "1"));
            
            #endregion
            
            response = httpClient.PostAsync(new Uri(url), new FormUrlEncodedContent(paramList)).Result;
            result = response.Content.ReadAsStringAsync().Result;*/

            #endregion

            #region 使用HttpWebRequest请求数据

            /*HttpWebRequest httpClient = WebRequest.Create(@"http://rds.aliyuncs.com") as HttpWebRequest;
            if (httpClient != null)
            {
                httpClient.Method = "GET";
                httpClient.ContentType = "application/xml";
            }
            if (httpClient != null)
            {
                using (Stream stream = httpClient.GetRequestStream())
                {
                    string httpContext = @"AccessKeyId=TMP.AQH86KnrYf6hOvDtbcOR2svc8F1zvxwJ5-b-97c1jT5oWvmY-o7Qe4cebJz5MC4CFQDxS_FvIHVBUvpgPGyLHVZiizd9owIVAOjqIBRGhx8_Dv-oDVQ0K5I1mhe5
                    &Action=DescribeSlowLogs
                    &DBInstanceId=rm-wz96sojqf5ffnmg5q
                    &EndTime=2017-07-05Z
                    &Format=JSON
                    &SecureTransport=true
                    &SignatureMethod=HMAC-SHA1
                    &SignatureNonce=58823de2d8abfa4408ed7672f7f96e93
                    &SignatureVersion=1.0
                    &SourceIp=183.128.132.156
                    &StartTime=2017-07-01Z
                    &Timestamp=2017-07-05T03%3A53%3A14Z
                    &Version=2014-08-15
                    &Signature=%2Bi72yFpuOXVnkC15vAko%2Fq5beXE%3D";
                    byte[] bytes = Encoding.UTF8.GetBytes(httpContext);
                    stream.Write(bytes,0,bytes.Length);
                    stream.Close();
                }               
                WebResponse response;
                try
                {
                    response = httpClient.GetResponse();
                    if (response.GetResponseStream() != null)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            String responseJsonString = streamReader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse) ex.Response;
                }
            }*/

            #endregion

        }

        /// <summary>
        /// 通过Get阿里云Rds慢日志Api测试
        /// </summary>
        [Fact]
        public void AliyunRdsSlowLogApiGetRequestTest()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("user-agent",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json,text/html");
            //Get请求测试           
            String responseExecute = CreateRequest().Execute("DescribeSlowLogs",
                new Dictionary<String, String>
                {
                    {"DBInstanceId", "rm-wz96sojqf5ffnmg5q"},
                    {"StartTime", "2017-07-01Z"},
                    {"EndTime", "2017-07-05Z"},
                    {"DBName", "dms_dev"},
                    {"SortKey", "TotalQueryTimes"},
                    {"PageSize", "100"},
                    {"PageNumber", "1"}
                });
            var rdsDescribeSlowLogsResponse = JsonConvert.DeserializeObject<RdsDescribeSlowLogsResponse>(responseExecute);

            if (rdsDescribeSlowLogsResponse.DBInstanceID == null)
            {
                var rdsErrorResponse = JsonConvert.DeserializeObject<RdsErrorResponse>(responseExecute);
                Assert.NotNull(rdsErrorResponse.Message);
            }
            else
            {
                Assert.NotNull(rdsDescribeSlowLogsResponse.DBInstanceID);
            }      
            
            #region JObject转换测试

            JObject result = JsonConvert.DeserializeObject(responseExecute) as JObject;
            if (result != null && result.Count == 4)
            {
                RdsErrorResponse rdsErrorResponse = new RdsErrorResponse
                {
                    RequestId = result.GetValue("RequestId").ToString(),
                    HostId = result.GetValue("HostId").ToString(),
                    Code = result.GetValue("Code").ToString(),
                    Message = result.GetValue("Message").ToString()
                };
                Assert.NotNull(rdsErrorResponse);
            }
            else
            {
                try
                {
                    if (result != null)
                    {
                        RdsDescribeSlowLogsResponse slowLogsResponse = new RdsDescribeSlowLogsResponse
                        {
                            PageNumber = long.Parse(result.GetValue("PageNumber").ToString()),
                            TotalRecordCount = long.Parse(result.GetValue("TotalRecordCount").ToString()),
                            RequestId = result.GetValue("RequestId").ToString(),
                            EndTime = DateTime.Parse(result.GetValue("EndTime").ToString()),
                            StartTime = DateTime.Parse(result.GetValue("StartTime").ToString()),
                            Engine = result.GetValue("Engine").ToString(),
                            PageRecordCount = long.Parse(result.GetValue("PageRecordCount").ToString())
                        };
                        Assert.NotNull(slowLogsResponse.DBInstanceID);
                    }
                    Assert.Null(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            #endregion

        }

        /// <summary>
        /// 根据应用名称获取查询慢日志整合数据单元测试
        /// </summary>
        [Fact]
        public void GetSqlSlowLogByAppNameTest()
        {
            var result = _service.GetSqlSlowLogByAppName("xiaobao", 7);
            Assert.NotNull(result.Data);
        }

        /// <summary>
        /// 根据筛选参数获取查询慢日志整合数据单元测试
        /// </summary>
        [Fact]
        public void GetSqlSlowLogByParameterTest()
        {
            IDictionary<string, object> parameter = new Dictionary<string, object>();
            parameter.Add(new KeyValuePair<string, object>(@"AppName","xiaobao"));
            parameter.Add(new KeyValuePair<string, object>(@"StartTime","2017-07-01Z"));
            parameter.Add(new KeyValuePair<string, object>(@"EndTime","2017-07-05Z"));
            var result = _service.GetSqlSlowLogByParameter(parameter);
            Assert.True(result.Data.Any());
        }

        /// <summary>
        /// 导出慢日志查询结果到Excel单元测试
        /// </summary>
        [Fact]
        public void ExportSqlSlowLogResultToExcelTest()
        {
            IDictionary<string, object> parameter = new Dictionary<string, object>();
            parameter.Add(new KeyValuePair<string, object>(@"AppName","xiaobao"));
            parameter.Add(new KeyValuePair<string, object>(@"StartTime","2017-07-15Z"));
            parameter.Add(new KeyValuePair<string, object>(@"EndTime","2017-07-17Z"));
            var exportSqlSlowLogResult = _service.GetExportSqlSlowLogResult(parameter);
            if (exportSqlSlowLogResult.Success)
            {
                var exportData = exportSqlSlowLogResult.Data;
                DataTable dataTable = DataTableHelper.ConvertEnumerableToDataTable(exportData);
                MemoryStream stream = (MemoryStream) _service.ExportSqlSlowLogResultToExcel(dataTable).Data;
                stream.Seek(0, SeekOrigin.Begin);
                FileStream fileStream = new FileStream(@"C:\导出慢日志查询结果到Excel单元测试.xls",FileMode.Create);
                stream.Position = 0;
                stream.WriteTo(fileStream);
                fileStream.Close();
            }
            else
            {
                Assert.True(false);
            }
        }

        #region 私有方法

        /// <summary>
        /// 创建Rds请求
        /// </summary>
        /// <returns></returns>
        private static RdsRequest CreateRequest() 
        {         
            string accessKeyId = ConfigHelper.GetConfigString("ACCESS_KEY_ID");
            string accessKeySecret = ConfigHelper.GetConfigString("ACCESS_KEY_SECRET");
            return new RdsRequest(accessKeyId, accessKeySecret);
        }

        #endregion
        
    }
}