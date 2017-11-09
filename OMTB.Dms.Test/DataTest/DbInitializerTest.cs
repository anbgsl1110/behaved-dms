using MySql.Data.Entity;
using System.Data.Entity;
using System.Linq;
using OMTB.Dms.Data.Database;
using Xunit;

namespace OMTB.Dms.Test.DataTest
{
    /// <summary>
    /// 数据库初始化
    /// </summary>
    public class DbInitializerTest
    {
        /// <summary>
        /// 删除并重新初始化数据库
        /// </summary>
        [Fact]
        public void DropAndCreateDatabase()
        {
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration()); //修正创建数据库时出错的问题
            DmsDbContext dbContext = new DmsDbContext();
            using (dbContext)
            {
                dbContext.Database.Delete();
                var count = dbContext.Log.Count();
                Assert.True(count > 0);
                var isExist = dbContext.Database.Exists();
                Assert.True(isExist);
            }
        }
    }
}