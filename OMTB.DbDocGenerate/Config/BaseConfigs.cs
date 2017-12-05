using System;
using System.Collections.Generic;
using System.Linq;

namespace DbDocGenerate.Config
{
    /// <summary>
    /// 基本设置类
    /// </summary>
    public class BaseConfigs
    {

        private static object lockHelper = new object();

        private static System.Timers.Timer baseConfigTimer = new System.Timers.Timer(15000);

        private static BaseConfigInfo m_configinfo;

        /// <summary>
        /// 静态构造函数初始化相应实例和定时器
        /// </summary>
        static BaseConfigs()
        {
            m_configinfo = BaseConfigFileManager.LoadConfig();
            baseConfigTimer.AutoReset = true;
            baseConfigTimer.Enabled = true;
            baseConfigTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            baseConfigTimer.Start();
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResetConfig();
        }


        /// <summary>
        /// 重设配置类实例
        /// </summary>
        public static void ResetConfig()
        {
            m_configinfo = BaseConfigFileManager.LoadRealConfig();
        }

        public static BaseConfigInfo GetBaseConfig()
        {
            return m_configinfo;
        }

        /// <summary>
        /// 保存配置实例
        /// </summary>
        /// <param name="baseconfiginfo"></param>
        /// <returns></returns>
        public static bool SaveConfig(BaseConfigInfo baseconfiginfo)
        {
            BaseConfigFileManager acfm = new BaseConfigFileManager();
            BaseConfigFileManager.ConfigInfo = baseconfiginfo;
            return acfm.SaveConfig();
        }


        /// <summary>
        /// 返回网站路径
        /// </summary>
        public static string GetWebPath
        {
            get { return GetBaseConfig().Webpath; }
        }

        public static string GetPageExt
        {
            get { return GetBaseConfig().PageExt; }
        }

        /// <summary>
        /// 返回数据库连接串
        /// </summary>
        public static string GetDBConnectString
        {
            get { return GetBaseConfig().Dbconnectstring; }
        }

        public static List<string> GetReadConnects
        {
            get { return GetBaseConfig().ReadConnects; }
        }

        /// <summary>
        /// 数据查询的连接串
        /// </summary>
        public static string GetReadConnectString
        {
            get
            {
                if (Config.BaseConfigs.GetReadConnects != null && Config.BaseConfigs.GetReadConnects.Count > 0)
                {
                    return Config.BaseConfigs.GetReadConnects[new Random().Next(0, Config.BaseConfigs.GetReadConnects.Count)];
                }
                else { return GetDBConnectString; }
            }
        }

        /// <summary>
        /// 返回表前缀
        /// </summary>
        public static string GetTablePrefix
        {
            get { return GetBaseConfig().Tableprefix; }
        }

        public static string GetDbType
        {
            get { return GetBaseConfig().Dbtype; }
        }

        public static int GetPlatform
        {
            get { return GetBaseConfig().Platform; }
        }

        public static int GetWebCode
        {
            get { return GetBaseConfig().WebCode; }
        }

        public static string GetAppSecret
        {
            get { return GetBaseConfig().AppSecret; }
        }

        public static string GetWebName
        {
            get { return GetBaseConfig().WebName; }
        }

        public static string GetWebUrl { get { return GetBaseConfig().WebUrl; } }

        public static string GetManageAreaName { get { return GetBaseConfig().ManageAreaName; } }

        public static string GetEmailStmp
        {
            get { return GetBaseConfig().EmailStmp; }
        }
        public static string GetEmailUser
        {
            get { return GetBaseConfig().EmailUser; }
        }
        public static string GetEmailPassword
        {
            get { return GetBaseConfig().EmailPassword; }
        }

        public static Boolean GetDebug
        {
            get { return GetBaseConfig().Debug; }
        }

        public static string GetCommonUrl
        {
            get { return GetBaseConfig().CommonUrl; }
        }

        public static string GetCDNManage { get { return GetBaseConfig().CDNManage; } }

        public static string GetCDNUpload { get { return GetBaseConfig().CDNUpload; } }

        /// <summary>
        /// 公共资源
        /// </summary>
        public static string GetCDNCommon { get { return GetBaseConfig().CDNCommon; } }

        public static string GetCDNImg { get { return GetBaseConfig().CDNImg; } }

        public static string GetCDNCss { get { return GetBaseConfig().CDNCss; } }

        public static string GetCDNJs { get { return GetBaseConfig().CDNJs; } }

        public static string GetCDNTime { get { return GetBaseConfig().CDNTime; } }

        public static string GetStaticWebUrl { get { return GetBaseConfig().StaticWebUrl; } }

        public static string GetUCWebUrl { get { return GetBaseConfig().UCWebUrl; } }

/*        public static HongWen.CacheManage.RedisConfigInfo GetDBCacheServer { get { return GetBaseConfig().DBCacheServer; } }

        public static HongWen.CacheManage.RedisConfigInfo GetUCRedisServer { get { return GetBaseConfig().UCRedisServer; } }

        public static HongWen.CacheManage.RedisConfigInfo GetAndroidRedisServer { get { return GetBaseConfig().AndroidRedisServer; } }*/

        public static string GetEwtUrl { get { return GetBaseConfig().EwtUrl; } }

        public static string GetKcUrl { get { return GetBaseConfig().KcUrl; } }

        public static string GetRedisVideoUrl { get { return GetBaseConfig().RedisVideoConnStr; } }

        public static string GetMongoDbCon { get { return GetBaseConfig().MongoDbConnectString; } }
        /// <summary>
        /// 总访问用户数系数
        /// </summary>
        public static decimal GetHotCourseUserFactor { get { return GetBaseConfig().HotCourseUserFactor; } }

        /// <summary>
        /// 总访问时长系数
        /// </summary>
        public static decimal GetHotCourseVisitTimesFactor { get { return GetBaseConfig().HotCourseVisitTimesFactor; } }

        /// <summary>
        /// 总评论数系数
        /// </summary>
        public static decimal GetHotCourseCommentFactor { get { return GetBaseConfig().HotCourseCommentFactor; } }

        /// <summary>
        /// 热门课程排行榜过滤课程ID字符串(首页)
        /// </summary>
        public static string GetHotCourseExceptedIDStringAll
        {
            get { return GetBaseConfig().HotCourseExceptIDStringAll; }
        }

        /// <summary>
        /// 热门课程排行榜过滤课程ID字符串(首页)
        /// </summary>
        public static string GetHotCourseExceptedIDStringSubject
        {
            get { return GetBaseConfig().HotCourseExceptIDStringSubject; }
        }

        /// <summary>
        /// 学科排名
        /// </summary>
        public static string GetSubjectRankingList
        {
            get { return GetBaseConfig().SubjectRankingList; }
        }

        /// <summary>
        /// 学科对照知识点
        /// </summary>
        public static string GetSubjectVsKnowledge
        {
            get { return GetBaseConfig().SubjectVsKnowledge; }
        }

        /// <summary>
        /// 智能推荐组件开关
        /// </summary>
        public static bool GetRecommendSwitch
        {
            get { return GetBaseConfig().RecommendEnabled == "1"; }
        }

        /// <summary>
        /// 统计代码url
        /// </summary>
        public static string GetStatisticsCodeUrl
        {
            get { return GetBaseConfig().StatisticsCodeUrl; }
        }

        public static List<string> GetLogApiConfig
        {
            get { return GetBaseConfig().LogApiConfig; }
        }


        /// <summary>
        /// 视频管理地址url
        /// </summary>
        public static string GetVideoManagerUrl
        {
            get { return GetBaseConfig().VideoManagerApiUrl; }
        }

        /// <summary>
        /// VMS视频接口地址
        /// </summary>
        public static string GetMediaSourceUrl
        {
            get { return GetBaseConfig().MediaSourceApiUrl; }
        }



        /// <summary>
        /// 直播api url
        /// </summary>
        public static string GetLiveApiUrl
        {
            get { return GetBaseConfig().LiveApiUrl; }
        }

        /// <summary>
        /// E 社区api url
        /// </summary>
        public static string GetBBSApiUrl
        {
            get { return GetBaseConfig().BBSApiUrl; }
        }

        public static string GetXinLiApiUrl
        {
            get { return GetBaseConfig().XinLiApiUrl; }
        }



        public static string GetRecmdConnString
        {
            get { return GetBaseConfig().RecmdConnString; }
        }

        public static string GetTiKu
        {
            get { return GetBaseConfig().TiKu; }
        }

        public static bool GetRecmdTopSwitch
        {
            get { return GetBaseConfig().RecmdTopSwitch == "1"; }
        }

        /*public static List<int> GetRecmdTop
        {
            get
            {
                return GetBaseConfig().RecmdTop.Split(',').Select(i => HongWen.Common.Utils.StrToInt(i, 0)).Where(i => i != 0).ToList();
            }
        }*/

        public static string GetTeacherApiUrl
        {
            get { return GetBaseConfig().TeacherApiUrl; }
        }
        public static string GetMessageCenterUrl
        {
            get { return GetBaseConfig().MessageCenterUrl; }
        }

    }
}