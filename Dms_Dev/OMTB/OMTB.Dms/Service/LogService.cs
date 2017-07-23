using OMTB.Dms.Data.Database;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.IService;

namespace OMTB.Dms.Service
{
    /// <summary>
    /// Log服务
    /// </summary>
    public class LogService : ILogService
    {
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="log">日志存储对象</param>
        public void AddLog(LogRepo log)
        {
            using (var dbContext = new DmsDbContext())
            {
                if (log != null)
                {
                    dbContext.Log.Add(log);
                    dbContext.SaveChanges();
                }               
            }
        }
    }
}