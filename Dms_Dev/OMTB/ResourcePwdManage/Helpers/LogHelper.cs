using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ResourcePwdManage
{
    public class LogHelper
    {

        #region 写文本日志
        /// <summary>
        /// 向日志文件中输出一个空行
        /// </summary>
        public void WriteSpaceLine()
        {
            WriteLog(string.Empty);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// /// <param name="logPath">日志路径</param>
        /// <param name="logName">日志文件名</param>
        private void WriteLog(string message, string logPath, string logName, Exception ex)
        {
            string logFile = Path.Combine(logPath, logName);
            //如果路径不存在，建立目录
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            //如果文件不存在，建立文件
            if (!File.Exists(logFile))
            {
                File.Create(logFile).Close();
            }


            try
            {
                var now = DateTime.Now.ToString("\r\n\r\n yyyy-MM-dd HH:mm:ss ~~~~~~~~~~~~~~ \r\n");
                WriteToFile(logFile, now + message + "\r\n");
                if (ex != null)
                {
                    WriteToFile(logFile, ex.Message + "\r\n");
                    WriteToFile(logFile, ex.StackTrace + "\r\n");
                }
            }
            catch
            {
                // do nothing
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// /// <param name="logPath">日志路径</param>
        public void WriteLog(string message, string logPath)
        {
            WriteLog(message, logPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log", null);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="sMessage">日志信息</param>
        public void WriteLog(string message)
        {
            WriteLog(message, AppDomain.CurrentDomain.BaseDirectory + "/log", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        }

        public void WriteLog(string message, Exception ex)
        {
            WriteLog(message, AppDomain.CurrentDomain.BaseDirectory + "/log", DateTime.Now.ToString("yyyy-MM-dd") + ".log", ex);
        }

        public void WriteLog(string message, string logPath, string logName)
        {
            string logFile = Path.Combine(logPath, logName);
            //如果路径不存在，建立目录
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            //如果文件不存在，建立文件
            if (!File.Exists(logFile))
            {
                File.Create(logFile).Close();
            }

            try
            {
                var now = DateTime.Now.ToString("\r\n\r\n yyyy-MM-dd HH:mm:ss ~~~~~~~~~~~~~~ \r\n");
                WriteToFile(logFile, now + message + "\r\n");
            }
            catch
            {
                // do nothing
            }
        }

        /// <summary>
        /// 把信息写到指定文件的尾部
        /// </summary>
        /// <param name="fileName">文件完整路径</param>
        /// <param name="msg">信息文本</param>
        public void WriteToFile(string fileName, string msg)
        {
            using (StreamWriter swWriter = new StreamWriter(new FileStream(fileName, FileMode.Append)))
            {
                swWriter.WriteLine(msg);
            }
        }
        #endregion

    }
}
