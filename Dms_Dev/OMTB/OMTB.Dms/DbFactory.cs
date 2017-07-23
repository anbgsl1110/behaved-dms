using System;
using System.Configuration;
using System.Data.Common;
using OMTB.Dms.Dto;

namespace OMTB.Dms
{
    public static class DbFactory
    {
        /// <summary>
        /// 获取执行SQL的数据库会话连接对象
        /// </summary>
        /// <returns></returns>
        public static DbConnection NewConnection(DbconnectInfo dbconnectInfo)
        {
            var factory = DbProviderFactories.GetFactory(dbconnectInfo.ProviderNameString);
            var connection = factory.CreateConnection();
            if (connection != null)
            {
                connection.ConnectionString = dbconnectInfo.ConnectionString;
                return connection;
            }
            throw new ArgumentException("数据库连接字符串");
        }

        /// <summary>
        /// 获取Dms数据库会话连接对象
        /// </summary>
        /// <returns></returns>
        public static DbConnection DmsConnection()
        {
            var connectionStrings = ConfigurationManager.ConnectionStrings["WriteDmsdev"];
            var factory = DbProviderFactories.GetFactory(connectionStrings.ProviderName);
            var connection = factory.CreateConnection();
            if (connection != null)
            {
                connection.ConnectionString = connectionStrings.ConnectionString;
                return connection;
            }
            throw new ArgumentException("数据库连接字符串");
        }
    }
}