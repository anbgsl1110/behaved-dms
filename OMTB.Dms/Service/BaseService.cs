namespace OMTB.Dms.Service
{
    /// <summary>
    /// Service基础类
    /// </summary>
    public class BaseService
    {
        public ConfigService ConfigService;
        
        public LogService LogService;

        public BaseService()
        {
            ConfigService = new ConfigService();
            LogService = new LogService();
        }
    }
}