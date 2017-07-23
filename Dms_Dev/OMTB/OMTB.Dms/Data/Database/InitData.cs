using System;
using System.Data.Entity;
using OMTB.Dms.Data.Entity;

namespace OMTB.Dms.Data.Database
{
    public class InitData : CreateDatabaseIfNotExists<DmsDbContext>
    {
        /// <summary>
        /// 数据库初始数据
        /// </summary>
        /// <param name="dbContext"></param>
        protected override void Seed(DmsDbContext dbContext)
        {         
            SetConfigInitData(ref dbContext);
            
            SetLogInitData(ref dbContext);

            SetRdsDbInstanceInfoData(ref dbContext);
        }

        /// <summary>
        /// 设置配置初始化数据
        /// </summary>
        /// <param name="dbContext"></param>
        public void SetConfigInitData(ref DmsDbContext dbContext)
        {
            #region 登录Dms系统时接收验证码的手机号码

            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "DefaultValidatePhone",
                ConfigValue = "18034648633",
                ConfigRemark = "",
                ConfigDescription = "登录Dms系统时接收验证码的手机号码",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });

            #endregion

            #region Dms执行的Sql字符中不能包含的敏感词

            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "update",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "insert",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "delete",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "replace",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "show",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "replace",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "alter",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "create",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "drop",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "rename",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "truncate",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "flush",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "grant",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "SqlSensitiveWords",
                ConfigValue = "revoke",
                ConfigRemark = "",
                ConfigDescription = "Dms执行的Sql字符中不能包含的敏感词",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });

            #endregion

            #region 允许访问的Ip地址

            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "IpWhiteAddress",
                ConfigValue = "127.0.0.1",
                ConfigRemark = "Ip白名单",
                ConfigDescription = "允许访问的Ip地址",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });
            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "IpWhiteAddress",
                ConfigValue = "::1",
                ConfigRemark = "Ip白名单",
                ConfigDescription = "允许访问的Ip地址",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });

            #endregion

            #region 待操作的数据库连接信息

            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "DbconnectInfo",
                ConfigValue = "server=114.215.158.176;port=5002;database=oemmis_dev;uid=oemmis_dev;pwd=000000;SslMode=None;",
                ConfigRemark = "MySql.Data.MySqlClient",
                ConfigDescription = "oemmis_dev",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });

            #endregion

            #region Rds慢日志管理

            dbContext.ConfigInfo.Add(new ConfigInfoRepo
            {
                ConfigName = "AliyunRdsApiUrl",
                ConfigValue = "rds.aliyuncs.com",
                ConfigRemark = "阿里云",
                ConfigDescription = "RDS API的服务接入地址",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            });            

            #endregion
        }

        /// <summary>
        /// 设置日志初始化数据
        /// </summary>
        /// <param name="dbContext"></param>
        public void SetLogInitData(ref DmsDbContext dbContext)
        {
            dbContext.Log.Add(new LogRepo
            {
                OperationType = "系统",
                OperationContext = "数据库初始化",
                Ip = "127.0.0.1",
                OperationResult = "初始化成功",
                OperationDate = DateTime.Now,
                Remark = ""
            });
        }
        
        /// <summary>
        /// 设置Rds实例信息初始化数据
        /// </summary>
        /// <param name="dbContext"></param>
        private void SetRdsDbInstanceInfoData(ref DmsDbContext dbContext)
        {           
            dbContext.RdsDbInstanceInfo.Add(new RdsDbInstanceInfoRepo
            {
                AppName = "xiaobao",
                DBInstanceId = "rm-wz96sojqf5ffnmg5q",
                DbName = "dms_dev"
            });
        }
    }
}