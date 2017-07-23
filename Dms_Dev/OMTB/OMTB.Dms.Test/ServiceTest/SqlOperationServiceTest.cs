using System.IO;
using OMTB.Dms.Dto;
using OMTB.Dms.Service;
using Xunit;

namespace OMTB.Dms.Test.ServiceTest
{
    public class SqlOperationServiceTest
    {
        private readonly SqlOperationService _sqlOperationService = new SqlOperationService();

        /// <summary>
        /// 获取Sql执行返回结果集单元测试
        /// </summary>
        [Fact]
        public void GetSqlExcuteResultTest()
        {
            string sql = @"";          
            ConfigService configService = new ConfigService();
            DbconnectInfo dbconnectInfo = configService.GetDbconnectInfoById(6).Data;
            
            //参数为空验证
            var result = _sqlOperationService.GetSqlExcuteResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //有限制关键字验证
            sql = @"Update";
            result = _sqlOperationService.GetSqlExcuteResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //SQl语法错误验证
            sql = @"hhh ds 1110;";
            result = _sqlOperationService.GetSqlExcuteResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //正常访问
            sql = @"SELECT * FROM log;";
            result = _sqlOperationService.GetSqlExcuteResult(sql, dbconnectInfo);
            Assert.NotNull(result.Data);
        }
        
        /// <summary>
        /// 获取导出结果集单元测试
        /// </summary>
        [Fact]
        public void GetExportResultTest()
        {
            string sql = @"";          
            ConfigService configService = new ConfigService();
            DbconnectInfo dbconnectInfo = configService.GetDbconnectInfoById(6).Data;
            
            //参数为空验证
            var result = _sqlOperationService.GetExportResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //有限制关键字验证
            sql = @"Update";
            result = _sqlOperationService.GetExportResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //SQl语法错误验证
            sql = @"hhh ds 1110;";
            result = _sqlOperationService.GetExportResult(sql, dbconnectInfo);
            Assert.Null(result.Data);
            
            //正常访问
            sql = @"SELECT * FROM user;";
            result = _sqlOperationService.GetExportResult(sql, dbconnectInfo);
            Assert.NotNull(result.Data);
        }

        /// <summary>
        /// 导出Sql执行返回结果到Excel单元测试
        /// </summary>
        [Fact]
        public void ExportSqlExcuteResultToExcelTest()
        {
            string sql = @"SELECT * FROM user;";          
            ConfigService configService = new ConfigService();
            DbconnectInfo dbconnectInfo = configService.GetDbconnectInfoById(6).Data;
            var result = _sqlOperationService.GetExportResult(sql, dbconnectInfo);
            
            //有数据时导出到excel
            if (result.Success)
            {
                MemoryStream stream = (MemoryStream)_sqlOperationService.ExportSqlExcuteResultToExcel(result.Data).Data;
                stream.Seek(0, SeekOrigin.Begin);
                FileStream fileStream = new FileStream(@"C:\导出Sql执行返回结果到Excel单元测试.xls", FileMode.Create);
                stream.Position = 0;
                stream.WriteTo(fileStream);
                fileStream.Close();
            }
        }
    }
}