using System.Collections.Generic;
using System.Linq;
using OMTB.Dms.Common;
using OMTB.Dms.Data.Database;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;
using OMTB.Dms.IService;

namespace OMTB.Dms.Service
{
    /// <summary>
    /// config服务
    /// </summary>
    public class ConfigService : IConfigService
    {
        public ServiceResult<ServiceStateEnum, string> GetDefaultValidatePhone()
        {
            using (var dbContext = new DmsDbContext())
            {
                var result = dbContext.ConfigInfo
                    .SingleOrDefault(p => p.Id > 0 && p.ConfigName.Equals("DefaultValidatePhone"));
                if (result != null)
                {
                    return ServiceResult.Create(true, ServiceStateEnum.Success, result.ConfigValue);
                }
                return ServiceResult.Create(false, ServiceStateEnum.IsNull, string.Empty);
            }           
        }

        public void AddConfig(ConfigInfoRepo configInfoRepo)
        {
            using (var dbContext = new DmsDbContext())
            {
                dbContext.ConfigInfo.Add(configInfoRepo);
                dbContext.SaveChanges();
            }
        }

        public ServiceResult<ServiceStateEnum, List<DbconnectInfo>> GetDbconnectInfoList()
        {
            using (var dbContext = new DmsDbContext())
            {
                var result = dbContext.ConfigInfo.Where(p => p.ConfigName.Equals("DbconnectInfo"))
                    .Select(t => new {t.Id, t.ConfigName, t.ConfigValue, t.ConfigRemark, t.ConfigDescription}).ToList();
                if (result.Any())
                {
                    List<DbconnectInfo> list = (from temp in result
                        select new DbconnectInfo
                        {
                            Id = temp.Id,
                            DbConnectName = temp.ConfigDescription,
                            ConnectionString = temp.ConfigValue,
                            ProviderNameString = temp.ConfigRemark
                        }).ToList();
                    return ServiceResult.Create(true, ServiceStateEnum.Success, list);
                }
                return ServiceResult.Create(false, ServiceStateEnum.IsNull, new List<DbconnectInfo>());
            }
        }

        public ServiceResult<ServiceStateEnum, DbconnectInfo> GetDbconnectInfoById(long id)
        {
            using (var dbContext = new DmsDbContext())
            {
                var result = dbContext.ConfigInfo.Where(p => p.Id == id)
                    .Select(t => new {t.Id, t.ConfigName, t.ConfigValue, t.ConfigRemark, t.ConfigDescription}).Single();
                if (result != null)
                {
                    DbconnectInfo dbconnectInfo = new DbconnectInfo
                        {
                            Id = result.Id,
                            DbConnectName = result.ConfigDescription,
                            ConnectionString = result.ConfigValue,
                            ProviderNameString = result.ConfigRemark
                        };
                    return ServiceResult.Create(true, ServiceStateEnum.Success, dbconnectInfo);
                }
                return ServiceResult.Create(false, ServiceStateEnum.IsNull, new DbconnectInfo());
            }
        }

        public ServiceResult<ServiceStateEnum, DbconnectInfo> GetDbconnectInfoByName(string dbConnectName)
        {
            using (var dbContext = new DmsDbContext())
            {
                var result = dbContext.ConfigInfo.Where(p => p.Id > 0 && p.ConfigDescription == dbConnectName)
                    .Select(t => new {t.Id, t.ConfigName, t.ConfigValue, t.ConfigRemark, t.ConfigDescription}).Single();
                if (result != null)
                {
                    DbconnectInfo dbconnectInfo = new DbconnectInfo
                    {
                        Id = result.Id,
                        DbConnectName = result.ConfigDescription,
                        ConnectionString = result.ConfigValue,
                        ProviderNameString = result.ConfigRemark
                    };
                    return ServiceResult.Create(true, ServiceStateEnum.Success, dbconnectInfo);
                }
                return ServiceResult.Create(false, ServiceStateEnum.IsNull, new DbconnectInfo());
            }
        }

        public ServiceResult<ServiceStateEnum, List<string>> GetSqlSensitiveWordsList()
        {
            using (var dbContext = new DmsDbContext())
            {
                var result = dbContext.ConfigInfo.Where(p => p.ConfigName.Equals("SqlSensitiveWords"))
                    .Select(t => t.ConfigValue.ToUpper()).ToList();
                return ServiceResult.Create(true, ServiceStateEnum.Success, result);
            }
        }

        public ServiceResult<ServiceStateEnum, IpWhiteInfo> GetIpWhiteInfo()
        {
            using (var dbContext = new DmsDbContext())
            {
                var ipWhiteAddressList = dbContext.ConfigInfo.Where(p => p.ConfigName.Equals("IpWhiteAddress"))
                    .Select(t => t.ConfigValue).ToList();
                IpWhiteInfo ipWhiteInfo = new IpWhiteInfo
                {
                    IpWhiteAddressList = ipWhiteAddressList
                };
                return ServiceResult.Create(true, ServiceStateEnum.Success, ipWhiteInfo);
            }
        }

        public ServiceResult<ServiceStateEnum, List<string>> GetAppInfoList()
        {
            using (DmsDbContext dbContext = new DmsDbContext())
            {
                var appNames = dbContext.RdsDbInstanceInfo.Where(p => p.Id > 0).GroupBy(p => p.AppName)
                    .Select(t => t.Key).ToList();
                if (appNames.Any())
                {
                    return ServiceResult.Create(true, ServiceStateEnum.Success, appNames);
                }
                return ServiceResult.Create(false, ServiceStateEnum.IsNull, new List<string>());
            }
        }

        public ServiceResult<ServiceStateEnum, List<RdsDbInstanceInfoRepo>> GetDbInstanceIdListByAppName(string appName)
        {
            using (DmsDbContext dbContext = new DmsDbContext())
            {
                var rdsDbInstanceInfos = dbContext.RdsDbInstanceInfo.Where(p => p.AppName == appName).ToList();
                return ServiceResult.Create(true, ServiceStateEnum.Success, rdsDbInstanceInfos);
            }
        }

        public ServiceResult<ServiceStateEnum, RdsDbInstanceInfoRepo> GetDbInstanceInfoByAppName(string appName)
        {
            using (DmsDbContext dbContext = new DmsDbContext())
            {
                var rdsDbInstanceInfo = dbContext.RdsDbInstanceInfo.FirstOrDefault(p => p.AppName == appName);
                return ServiceResult.Create(true, ServiceStateEnum.Success, rdsDbInstanceInfo);
            }
        }

        public ServiceResult<ServiceStateEnum, string> GetAliyunRdsApiUrlString()
        {
            using (var dbContext = new DmsDbContext())
            {
                string aliyunRdsApiUrl = dbContext.ConfigInfo.Where(p => p.ConfigName.Equals("AliyunRdsApiUrl"))
                    .Select(t => t.ConfigValue).FirstOrDefault();

                return ServiceResult.Create(true, ServiceStateEnum.Success, aliyunRdsApiUrl);
            }
        }
    }
}