using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using DbDocGenerate.Config;

namespace DbDocGenerate.Plugin
{
    public class LogManager
    {
        private static string logPath = string.Empty;
        private static readonly object locker = new object();

        private static string logFielPrefix = string.Empty;

        /// <summary>
        ///     保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    //if (System.Web.HttpContext.Current == null)
                    // Windows Forms 应用
                    logPath = AppDomain.CurrentDomain.BaseDirectory;
                    //else
                    // Web 应用
                    //logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                }
                if (logPath.Substring(logPath.Length - 1) == "\\" || logPath.Substring(logPath.Length - 1) == "/")
                {
                    return logPath;
                }
                logPath = logPath + @"\";
                return logPath;
            }
            set { logPath = value; }
        }

        /// <summary>
        ///     日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        ///     写日志
        /// </summary>
        public static void WriteLog(string logFile, string msg, bool only)
        {
            WriteLog(logFile, msg, only, Encoding.GetEncoding("GB2312"), ".log");
        }

        public static void WriteLog(string LogFile, string msg, bool only, Encoding encoding, string fileExt)
        {
            //try
            //{
            lock (locker)
            {
                /*
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    LogPath + LogFielPrefix + logFile + " " +
                    DateTime.Now.ToString("yyyyMMdd") + ".Log"
                    );
                 */

                //added by xuc 2015.07.14   增加日志文件是否存在判断，避免文件不存在报错
                var filePath = LogPath + LogFielPrefix + LogFile + " " + DateTime.Now.ToString("yyyyMMdd") + fileExt;
                if (!File.Exists(filePath))
                {
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);

                    File.Create(filePath).Close();
                }


                var sw = new StreamWriter(filePath, true, encoding);

                if (only)
                {
                    sw.WriteLine(msg);
                }
                else
                {
                    sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: ") + msg);
                }
                sw.Close();
            }
            //}
            //catch
            //{ }
        }

        public static void WriteLog(string logFile, string msg)
        {
            WriteLog(logFile, msg, false);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="msg"></param>
        /// <param name="memo"></param>
        /// <param name="noOnlineLog">选择本地日志还是线上日志，默认false为线上日志,</param>
        public static void WriteLog(LogFile logFile, string msg, string memo = "", bool noOnlineLog = false, string source = "", string trace = "", string siteName = "")
        {
            if (string.IsNullOrEmpty(msg)) return;  //过滤空白日志无效动作

            //hw.config  Debug  设置为true  写入本地   false： 写入线上日志库  added by xuc 2016-4-27
            //if (Study.Config.BaseConfigs.GetDebug || noOnlineLog)
            //{
                if (!string.IsNullOrEmpty(msg))
                    WriteLog(logFile.ToString(), msg);

                if (!string.IsNullOrEmpty(source))
                    WriteLog(logFile.ToString(), source);

                if (!string.IsNullOrEmpty(trace))
                    WriteLog(logFile.ToString(), trace);

                if (!string.IsNullOrEmpty(siteName))
                    WriteLog(logFile.ToString(), siteName);
            //}
            //else
            //    WriteOnlineLog(logFile, msg, memo, source, trace, siteName);
        }

        public static void WriteLog(int index, LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString() + index, msg);
        }

        #region 线上日志统一调用http接口

        private static bool isApiNetworkEnabled
        {
            get { return Http.IsUrlEnabled(BaseConfigs.GetLogApiConfig[0]); }
        }


        protected static void WriteOnlineLog(LogFile logFile, string msg, string memo = "", string source = "", string trace = "", string siteName = "")
        {
            if (isApiNetworkEnabled)
            {
                WriteOnlineLogWithWebApi(logFile, msg, memo, source, trace, siteName);
            }
            //线上日志不可写则写本地日志
            else
            {
                WriteLog(logFile, msg, memo, true, source, trace, siteName);
            }
        }

        #region 通过webapi接口写入错误日志


        public static string WriteAppLog(string apiname, string data, string headerstr, int type)
        {
            string LogText = "";
            try
            {
                string Path = HttpContext.Current.Server.MapPath("\\ExceLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_App.log");

                if (type == 1)
                {
                    LogText += "-------------------------------------------------------------------\r\n";
                    LogText += "\r\n-----Start \r\n";
                }
                else
                {
                    LogText += "\r\n-----Result \r\n";
                }

                LogText += "Time:" + System.DateTime.Now + "\r\n"
                                + "ApiName:" + apiname + "\r\n"
                                + "Data:" + data + "\r\n"
                                + "Headers:" + headerstr + "\r\n\n";

                LogManager.WriteLog(LogFile.Error, LogText);
                //FilesHelper.WriteStrToTxtFile(Path, LogText, FileMode.Append);
            }
            catch (Exception e)
            {

            }
            return LogText;
        }


        /// <summary>
        /// 写日志 扩展方法，针对Exception 合并
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="ex"></param>
        /// <param name="memo">如:当前请求地址等无法从ex中获取的信息</param>
        /// <param name="noOnlineLog"></param>
        public static void WriteLog(LogFile logFile, Exception ex, string memo = "", bool noOnlineLog = false)
        {
            //hw.config  Debug  设置为true  写入本地   false： 写入线上日志库  added by xuc 2016-4-27
            //if (Study.Config.BaseConfigs.GetDebug || noOnlineLog)
            //{
                if (!string.IsNullOrEmpty(memo))
                    WriteLog(logFile.ToString(), memo);

                WriteLog(logFile.ToString(), ex.Message);
                WriteLog(logFile.ToString(), ex.Source);
                WriteLog(logFile.ToString(), ex.StackTrace);
                WriteLog(logFile.ToString(), ex.TargetSite.Name);
            //}
            //else
            //    WriteOnlineLog(logFile, ex.Message, memo, ex.Source, ex.StackTrace, ex.TargetSite.Name);
        }


        /// <summary>
        /// 通过webpai
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="msg"></param>
        /// <param name="memo"></param>
        /// <param name="source"></param>
        /// <param name="trace"></param>
        /// <param name="siteName"></param>
        protected static void WriteOnlineLogWithWebApi(LogFile logFile, string msg, string memo = "", string source = "", string trace = "", string siteName = "")
        {
            var logApiConfig = BaseConfigs.GetLogApiConfig;
            var logLevel = GetLogLevel(logFile);
            /*var logApp = Utils.StrToInt(logApiConfig[1], 0);
            var productLine = Utils.StrToInt(logApiConfig[2], 0);*/

            var http = new Http();
            string url = BaseConfigs.GetLogApiConfig[0];
            http.PageEncode = Encoding.GetEncoding("UTF-8");
            var postData = new Dictionary<string, string>();
            postData.Add(LogApiParam.level.ToString(), logLevel.ToString());
            /*postData.Add(LogApiParam.type.ToString(), logApp.ToString());
            postData.Add(LogApiParam.line.ToString(), productLine.ToString());*/
            postData.Add(LogApiParam.log.ToString(), msg);
            postData.Add(LogApiParam.memo.ToString(), memo);
            /*postData.Add(LogApiParam.date.ToString(), UnixDateTimeHelper.ConvertToUnixTimestamp(DateTime.Now).ToString());*/
            postData.Add(LogApiParam.source.ToString(), source);
            postData.Add(LogApiParam.trace.ToString(), trace);
            postData.Add(LogApiParam.site.ToString(), siteName);
            string method = "POST";
            http.GetHtml(postData, url, method);
        }

        private static int GetLogLevel(LogFile logFile)
        {
            if (Enum.IsDefined(typeof(LogLevel), logFile.ToString()))
            {
                return (int)Enum.Parse(typeof(LogLevel), logFile.ToString());
            }

            if (LogFile.Data == logFile)
                return (int)LogLevel.Sql;
            if (LogFile.Warning == logFile)
                return (int)LogLevel.Warning;
            if (LogFile.Error == logFile)
                return (int)LogLevel.Error;
            if (LogFile.Trace == logFile)
                return (int)LogLevel.Debug;

            return (int)LogLevel.Info;
        }
        #endregion

        #endregion

        /// <summary>
        /// logapi传递参数
        /// </summary>
        public enum LogApiParam
        {
            level,
            type,
            line,
            log,
            memo,
            date,
            source,
            trace,
            site
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogFile
    {
        Html,
        Trace,
        Warning,
        Error,
        Data,
        PiCi,
        MobileError,
        MobileSucc,
        Log
    }

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// SQL耗时
        /// </summary>
        Sql = 1,

        /// <summary>
        /// 调试信息，
        /// </summary>
        Debug = 2,

        /// <summary>
        /// 反馈系统当前状态 
        /// </summary>
        Info = 3,

        /// <summary>
        /// 警告
        /// </summary>
        Warning = 4,

        /// <summary>
        /// 错误 最高错误等级，引起系统停止工作
        /// </summary>
        Error = 5
    }
}