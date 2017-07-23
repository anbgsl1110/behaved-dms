using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourcePwdManage.Entity
{
    public class ECS
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string WanIp { get; set; }

        public string InternalIp { get; set; }

        public int OS { get; set; }

        public string DomainName { get; set; }

        public short Provider { get; set; }

        public string InstanceId { get; set; }

        public string InstanceName { get; set; }

        public string Description { get; set; }

        public string Remark { get; set; }
    }

}