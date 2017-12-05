using System;
using System.Collections.Generic;

namespace DbDocGenerate.Config
{
    /// <summary>
    /// 基本设置描述类, 加[Serializable]标记为可序列化
    /// </summary>
    [Serializable]
    public class BaseConfigInfo : IConfigInfo
    {
        #region 私有字段

        private string m_dbconnectstring = "Data Source=;User ID=;Password=;Initial Catalog=;Pooling=true";		// 数据库连接串-格式(中文为用户修改的内容)：Data Source=数据库服务器地址;User ID=您的数据库用户名;Password=您的数据库用户密码;Initial Catalog=数据库名称;Pooling=true
        private string m_tableprefix = "eb_";		// 数据库中表的前缀
        private string m_webpath = "/";			// 论坛在站点内的路径
        private string m_dbtype = "";
        private string m_pageext = ".aspx";

        private string m_webname = "网站名称";
        private string m_emailstmp = "";
        private string m_emailuser = "";
        private string m_emailpassword = "";

        #endregion

        #region 属性

        /// <summary>
        /// 数据库连接串
        /// 格式(中文为用户修改的内容)：
        ///    Data Source=数据库服务器地址;
        ///    User ID=您的数据库用户名;
        ///    Password=您的数据库用户密码;
        ///    Initial Catalog=数据库名称;Pooling=true
        /// </summary>
        public string Dbconnectstring
        {
            get
            {
                return m_dbconnectstring;
                // return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + System.Web.HttpContext.Current.Server.MapPath(Webpath) + m_dbconnectstring + ";Persist Security Info=True;";
            }
            set { m_dbconnectstring = value; }
        }

        public List<string> ReadConnects { get; set; }



        /// <summary>
        /// 数据库中表的前缀
        /// </summary>
        public string Tableprefix
        {
            get { return m_tableprefix; }
            set { m_tableprefix = value; }
        }

        /// <summary>
        /// 站点内的路径
        /// </summary>
        public string Webpath
        {
            get { return m_webpath; }
            set { m_webpath = value; }
        }

        public string PageExt
        {
            get { return m_pageext; }
            set { m_pageext = value; }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string Dbtype
        {
            get { return m_dbtype; }
            set { m_dbtype = value; }
        }

        /// <summary>
        /// 平台代码
        /// </summary>
        public int Platform { get; set; }

        /// <summary>
        /// 网站代码
        /// </summary>
        public int WebCode { get; set; }

        /// <summary>
        /// 网站密钥
        /// </summary>
        public string AppSecret { get; set; }

        public string WebName
        {
            get { return m_webname; }
            set { m_webname = value; }
        }

        public string WebUrl { get; set; }

        public string ManageAreaName { get; set; }


        public string EmailStmp
        {
            get { return m_emailstmp; }
            set { m_emailstmp = value; }
        }
        public string EmailUser
        {
            get { return m_emailuser; }
            set { m_emailuser = value; }
        }
        public string EmailPassword
        {
            get { return m_emailpassword; }
            set { m_emailpassword = value; }
        }

        public Boolean Debug { get; set; }

        /// <summary>
        /// kc 正式 测试地址
        /// </summary>
        public string KcUrl { get; set; }

        /// <summary>
        /// ewt 正式 测试地址
        /// </summary>
        public string EwtUrl { get; set; }

        /// <summary>
        /// 公共文件，JS CSS IMG
        /// </summary>
        public string CommonUrl { get; set; }

        /// <summary>
        /// 管理后台公共文件，JS CSS IMG
        /// </summary>
        public string CDNManage { get; set; }

        /// <summary>
        /// 上传文件
        /// </summary>
        public string CDNUpload { get; set; }

        /// <summary>
        /// 公共资源
        /// </summary>
        public string CDNCommon { get; set; }

        /// <summary>
        /// 前端图片
        /// </summary>
        public string CDNImg { get; set; }

        /// <summary>
        /// 前端样式
        /// </summary>
        public string CDNCss { get; set; }

        /// <summary>
        /// 前端脚本
        /// </summary>
        public string CDNJs { get; set; }

        /// <summary>
        /// JS,CSS 清除缓存的time
        /// </summary>
        public string CDNTime { get; set; }

        /// <summary>
        /// 资源网站URL
        /// </summary>
        public string StaticWebUrl { get; set; }

        /// <summary>
        /// 用户中心网站URL
        /// </summary>
        public string UCWebUrl { get; set; }

/*        /// <summary>
        /// 用户中心Redis地址
        /// </summary>
        public HongWen.CacheManage.RedisConfigInfo UCRedisServer { get; set; }

        /// <summary>
        /// DBCache地址
        /// </summary>
        public HongWen.CacheManage.RedisConfigInfo DBCacheServer { get; set; }

        /// <summary>
        /// Andorid登陆状态
        /// </summary>
        public HongWen.CacheManage.RedisConfigInfo AndroidRedisServer { get; set; }*/

        /// <summary>
        /// 热门课程包用户访问系数
        /// <createdate>added by xuc 2015.10.13</createdate>
        /// </summary>
        public decimal HotCourseUserFactor { get; set; }

        /// <summary>
        /// 热门课程包总访问时长系数 
        /// <createdate>added by xuc 2015.10.13</createdate>
        /// </summary>
        public decimal HotCourseVisitTimesFactor { get; set; }

        /// <summary>
        /// 热门课程包总评论数系数
        /// <createdate>added by xuc 2015.10.13</createdate>
        /// </summary>
        public decimal HotCourseCommentFactor { get; set; }

        /// <summary>
        /// 热门课程后台干预删除的首页排行榜课程包ID字符串
        /// <createdate>added by xuc 2015.10.14</createdate>
        /// </summary>
        public string HotCourseExceptIDStringAll { get; set; }

        /// <summary>
        /// 热门课程后台干预删除的各学科排行榜课程包ID字符串
        /// </summary>
        public string HotCourseExceptIDStringSubject { get; set; }

        /// <summary>
        /// 学科排名
        /// </summary>
        public string SubjectRankingList { get; set; }

        /// <summary>
        /// redis连接
        /// </summary>
        public string RedisVideoConnStr { get; set; }

        /// <summary>
        /// mongodb连接
        /// </summary>
        public string MongoDbConnectString { get; set; }

        /// <summary>
        /// 学科对照知识点
        /// </summary>
        public string SubjectVsKnowledge { get; set; }

        /// <summary>
        /// 智能推荐组件开关
        /// </summary>
        public string RecommendEnabled { get; set; }

        /// <summary>
        /// 统计代码url added by xuc 2016.02.22
        /// </summary>
        public string StatisticsCodeUrl { get; set; }

        public List<string> LogApiConfig { get; set; }

        /// <summary>
        /// 视频管理接口地址
        /// </summary>
        public string VideoManagerApiUrl { get; set; }

        /// <summary>
        /// VMS视频接口地址
        /// </summary>
        public string MediaSourceApiUrl { get; set; }

        /// <summary>
        /// 直播接口地址
        /// </summary>
        public string LiveApiUrl { get; set; }

        /// <summary>
        /// E社区接口地址(BBS)
        /// </summary>
        public string BBSApiUrl { get; set; }

        /// <summary>
        /// 心理接口地址
        /// </summary>
        public string XinLiApiUrl { get; set; }

        /// <summary>
        /// 课程推荐数据库连接字符串
        /// </summary>
        public string RecmdConnString { get; set; }

        /// <summary>
        /// 题库数据库连接字符串，暂时的
        /// </summary>
        public string TiKu { get; set; }

        /// <summary>
        /// www首页猜你喜欢 置顶推荐课程包功能开关
        /// </summary>
        public string RecmdTopSwitch { get; set; }

        /// <summary>
        /// www首页猜你喜欢 置顶推荐课程包ID以及排序
        /// </summary>
        public string RecmdTop { get; set; }

        /// <summary>
        /// 教师端接口地址
        /// </summary>
        public string TeacherApiUrl { get; set; }
        /// <summary>
        /// 消息中心接口地址
        /// </summary>
        public string MessageCenterUrl { get; set; }

        #endregion

    }
}
