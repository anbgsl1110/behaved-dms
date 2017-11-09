using OMTB.Dms.Data.Entity;

namespace OMTB.Dms.IService
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogService : IBaseService
    {
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <returns></returns>
        void AddLog(LogRepo log);
    }
}