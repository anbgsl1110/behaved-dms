using System;
using OMTB.Dms.Data.Entity;
using OMTB.Dms.Service;
using Xunit;

namespace OMTB.Dms.Test.ServiceTest
{
    /// <summary>
    /// 日志service单元测试
    /// </summary>
    public class LogServiceTest
    {
        private readonly LogService _logService = new LogService();

        /// <summary>
        /// 插入操作日志单元测试
        /// </summary>
        [Fact]
        public void AddLogTest()
        {
            _logService.AddLog(new LogRepo
            {
                OperationType = "单元测试",
                OperationContext = "",
                Ip = "127.0.0.1",
                OperationResult = "",
                OperationDate = DateTime.Now,
                Remark = ""
            });
        }
    }
}