using System.Collections.Generic;
using OMTB.Dms.Common;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;

namespace OMTB.Dms.IService
{
    /// <summary>
    /// ConfigService接口
    /// </summary>
    public interface IConfigService : IBaseService
    {
        #region SQL操作

        /// <summary>
        /// 获取默认验证手机号码
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum,string> GetDefaultValidatePhone();

        /// <summary>
        /// 添加Config信息
        /// </summary>
        /// <param name="configInfoRepo"></param>
        void AddConfig(ConfigInfoRepo configInfoRepo);

        /// <summary>
        /// 获取数据库连接信息集合
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<DbconnectInfo>> GetDbconnectInfoList();

        /// <summary>
        /// 根据Id获取数据库连接信息
        /// </summary>
        /// <param name="id">配置Id</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, DbconnectInfo> GetDbconnectInfoById(long id);
        
        /// <summary>
        /// 根据dbConnectName获取数据库连接信息
        /// </summary>
        /// <param name="dbConnectName">数据库连接名称</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, DbconnectInfo> GetDbconnectInfoByName(string dbConnectName);

        /// <summary>
        /// 获取SQL执行敏感词集合
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<string>> GetSqlSensitiveWordsList();

        /// <summary>
        /// 获取Ip白名单信息
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, IpWhiteInfo> GetIpWhiteInfo();

        #endregion

        #region Rds慢日志整合管理

        /// <summary>
        /// 获取应用名称列表
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<string>> GetAppInfoList();

        /// <summary>
        /// 根据应用名称获取该应用的全部数据库实例集合
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, List<RdsDbInstanceInfoRepo>> GetDbInstanceIdListByAppName(string appName);

        /// <summary>
        /// 根据数据库实例Id获取该数据库实例的配置信息
        /// </summary>
        /// <param name="appName">应用名称</param>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, RdsDbInstanceInfoRepo> GetDbInstanceInfoByAppName(string appName);

        /// <summary>
        /// 获取阿里云RDS API的服务接入地址
        /// </summary>
        /// <returns></returns>
        ServiceResult<ServiceStateEnum, string> GetAliyunRdsApiUrlString();

        #endregion

    }
}