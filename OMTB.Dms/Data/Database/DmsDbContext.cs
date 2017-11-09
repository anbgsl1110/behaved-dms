using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Dto;

namespace OMTB.Dms.Data.Database
{
    public class DmsDbContext : DbContext
    {
        static DmsDbContext()
        {
            System.Data.Entity.Database.SetInitializer(new InitData());
        }

        public DmsDbContext() : base("WriteDmsdev")
        {
        }

        /// <summary>
        /// 重写数据库表格模型
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); //移除复数表名

            modelBuilder.Entity<LogRepo>().ToTable("log");
            modelBuilder.Entity<LogRepo>().Property(p => p.Ip).HasMaxLength(50);
            modelBuilder.Entity<LogRepo>().Property(p => p.OperationContext).HasMaxLength(500);
            modelBuilder.Entity<LogRepo>().Property(p => p.OperationResult).HasMaxLength(50);
            modelBuilder.Entity<LogRepo>().Property(p => p.Remark).HasMaxLength(200);
            modelBuilder.Entity<LogRepo>().Property(p => p.OperationType).HasMaxLength(50);

            modelBuilder.Entity<ConfigInfoRepo>().ToTable("config");
            modelBuilder.Entity<ConfigInfoRepo>().Property(p => p.ConfigName).HasMaxLength(50);
            modelBuilder.Entity<ConfigInfoRepo>().Property(p => p.ConfigValue).HasMaxLength(500);
            modelBuilder.Entity<ConfigInfoRepo>().Property(p => p.ConfigDescription).HasMaxLength(100);
            modelBuilder.Entity<ConfigInfoRepo>().Property(p => p.ConfigRemark).HasMaxLength(200);

            modelBuilder.Entity<RdsSlowLogRequestRepo>().ToTable("rdsslowlogrequest");

            modelBuilder.Entity<SqlSlowLogRepo>().ToTable("sqlslowlog");

            modelBuilder.Entity<RdsDbInstanceInfoRepo>().ToTable("rdsDbInstanceInfo");
        }
        
        /// <summary>
        /// 操作日志
        /// </summary>
        public IDbSet<LogRepo> Log { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public IDbSet<ConfigInfoRepo> ConfigInfo { get; set; }
        
        /// <summary>
        /// Rds慢日志请求
        /// </summary>
        public IDbSet<RdsSlowLogRequestRepo> RdsSlowLogRequest { get; set; }
        
        /// <summary>
        /// Mysql慢日志
        /// </summary>
        public IDbSet<SqlSlowLogRepo> SqlSlowLog { get; set; }
        
        /// <summary>
        /// Rds实例信息
        /// </summary>
        public IDbSet<RdsDbInstanceInfoRepo> RdsDbInstanceInfo { get; set; }
    }
}