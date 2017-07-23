using System;
using log4net;

namespace OMTB.Component.Util
{
    public class LogHelper
    {
        public static void WriteLog(Exception ex)
        {
            var log = LogManager.GetLogger("Loggering");
            log.Error("Error", ex);
        }

        public static void WriteLog(string msg)
        {
            var log = LogManager.GetLogger("Loggering");
            log.Error(msg);
        }
    }
}
