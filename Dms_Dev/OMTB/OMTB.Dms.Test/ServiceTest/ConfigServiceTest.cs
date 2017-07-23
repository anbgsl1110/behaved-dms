using OMTB.Dms.Data.Entity;
using OMTB.Dms.Service;
using Xunit;

namespace OMTB.Dms.Test.ServiceTest
{
    public class ConfigServiceTest
    {
        private readonly ConfigService _configService = new ConfigService();
        
        /// <summary>
        /// 获取默认验证手机号码单元测试
        /// </summary>
        [Fact]
        public void GetDefaultValidatePhoneTest()
        {
            var result = _configService.GetDefaultValidatePhone();
            Assert.Equal(result.Data,"18034648633");
        }

        /// <summary>
        /// 添加Config信息单元测试
        /// </summary>
        [Fact]
        public void AddConfigTest()
        {
/*            var configInfoRepo = new ConfigInfoRepo
            {
                ConfigName = "DbconnectInfo",
                ConfigValue =
                    @"server=rm-wz96sojqf5ffnmg5qo.mysql.rds.aliyuncs.com;port=3306;database=dms_dev;uid=dms_dev;pwd=JiaYe1110;SslMode=None;",
                ConfigRemark = "MySql.Data.MySqlClient",
                ConfigDescription = "rds_dms",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            };
            _configService.AddConfig(configInfoRepo);*/

            #region 开发过程中单元测试在数据库插入配置数据

            var configRepo1 = new ConfigInfoRepo
            {
                ConfigName = "xiaobao",
                ConfigValue = "rm-wz96sojqf5ffnmg5q",
                ConfigRemark = "dms_dev",
                ConfigDescription = "RdsAppInfo",
                IsValidate = true,
                IsModify = false,
                IsDelete = false
            };
            _configService.AddConfig(configRepo1);

            #endregion
            
        }

        /// <summary>
        /// 获取数据库连接信息集合单元测试
        /// </summary>
        [Fact]
        public void GetDbconnectInfoListTest()
        {
            var result = _configService.GetDbconnectInfoList();
            Assert.True(result.Success);
        }

        /// <summary>
        /// 根据Id获取数据库连接信息单元测试
        /// </summary>
        [Fact]
        public void GetDbconnectInfoByIdTest()
        {
            var result = _configService.GetDbconnectInfoById(6);
            Assert.NotNull(result.Data.Id);
        }
        
        /// <summary>
        /// 根据dbConnectName获取数据库连接信息单元测试
        /// </summary>
        [Fact]
        public void GetDbconnectInfoByNameTest()
        {
            var result = _configService.GetDbconnectInfoByName("dms_dev");
            Assert.NotNull(result.Data.Id);
        }

        /// <summary>
        /// 获取SQL执行敏感词集合单元测试
        /// </summary>
        [Fact]
        public void GetSqlSensitiveWordsListTest()
        {
            var result = _configService.GetSqlSensitiveWordsList();
            Assert.True(result.Data.Count > 0);
        }

        /// <summary>
        /// 获取Ip白名单信息单元测试
        /// </summary>
        [Fact]
        public void GetIpWhiteInfoTest()
        {
            var result = _configService.GetIpWhiteInfo();
            Assert.True(result.Data.IpWhiteAddressList.Count > 0);
        }

        /// <summary>
        ///  获取阿里云RDS API的服务接入地址单元测试
        /// </summary>
        [Fact]
        public void GetAliyunRdsApiUrlStringTest()
        {
            var result = _configService.GetAliyunRdsApiUrlString();
            Assert.True(result.Success);
        }

        /// <summary>
        /// 根据应用名称获取该应用的全部数据库实例集合
        /// </summary>
        [Fact]
        public void GetDbInstanceIdListByAppNameTest()
        {
            var result = _configService.GetDbInstanceIdListByAppName(@"xiaobao");
            Assert.True(result.Data.Count > 0);
        }

        /// <summary>
        /// 根据数据库实例Id获取该数据库实例的配置信息单元测试
        /// </summary>
        [Fact]
        public void GetDbInstanceInfoByAppNameTest()
        {
            var result = _configService.GetDbInstanceInfoByAppName(@"xiaobao");
            Assert.NotNull(result.Data);
        }
    }
}