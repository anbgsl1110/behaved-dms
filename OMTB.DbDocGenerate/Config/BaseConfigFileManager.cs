using System;
using System.Configuration;
using System.IO;
using System.Web;
using DbDocGenerate.Common;

namespace DbDocGenerate.Config
{

    /// <summary>
    /// 基本设置信息管理类
    /// </summary>
    class BaseConfigFileManager : DefaultConfigFileManager
    {
        private static BaseConfigInfo m_configinfo;

        /// <summary>
        /// 锁对象
        /// </summary>
        private static object m_lockHelper = new object();

        /// <summary>
        /// 文件修改时间
        /// </summary>
        private static DateTime m_fileoldchange;


        /// <summary>
        /// 初始化文件修改时间和对象实例
        /// </summary>
        static BaseConfigFileManager()
        {
            m_fileoldchange = System.IO.File.GetLastWriteTime(ConfigFilePath);
            m_configinfo = (BaseConfigInfo)DefaultConfigFileManager.DeserializeInfo(ConfigFilePath, typeof(BaseConfigInfo));
        }

        /// <summary>
        /// 当前配置类的实例
        /// </summary>
        public new static IConfigInfo ConfigInfo
        {
            get { return m_configinfo; }
            set { m_configinfo = (BaseConfigInfo)value; }
        }
        /// <summary>
        /// 配置文件所在路径
        /// </summary>
        public static string filename = null;


        /// <summary>
        /// 获取配置文件所在路径
        /// </summary>
        public new static string ConfigFilePath
        {
            get
            {
                if (filename == null)
                {
                    HttpContext context = HttpContext.Current;
                    if (context != null)
                    {
                        filename = context.Server.MapPath(ConfigurationManager.ConnectionStrings["configfile"].ConnectionString );
                        
                       // filename = context.Server.MapPath("/EB.config");
                    }
                    else
                    {
                        filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HW.config");
                    }

                    if (!File.Exists(filename))
                    {
                        throw new ExceptionHelper("发生错误: 网站根目录下没有正确的HW.config文件");
                    }
                }
                return filename;
            }
        }

        /// <summary>
        /// 加载配置类
        /// </summary>
        /// <returns></returns>
        public static BaseConfigInfo LoadConfig()
        {
            ConfigInfo = DefaultConfigFileManager.LoadConfig(ref m_fileoldchange, ConfigFilePath, ConfigInfo);
            return ConfigInfo as BaseConfigInfo;
        }

        /// <summary>
        /// 加载真正有效的配置类
        /// </summary>
        /// <returns></returns>
        public static BaseConfigInfo LoadRealConfig()
        {
            ConfigInfo = DefaultConfigFileManager.LoadConfig(ref m_fileoldchange, ConfigFilePath, ConfigInfo, false);
            return ConfigInfo as BaseConfigInfo;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        public override bool SaveConfig()
        {
            return base.SaveConfig(ConfigFilePath, ConfigInfo);
        }
    }
}
