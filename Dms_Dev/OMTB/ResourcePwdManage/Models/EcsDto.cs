using ResourcePwdManage.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourcePwdManage.Models
{
    public class EcsListSeachDto
    {
        public string Name { get; set; }

        public string WanIp { get; set; }

        public string InternalIp { get; set; }

        public OS OS { get; set; }

        public string DomainName { get; set; }

        public Provider Provider { get; set; }

        public string InstanceId { get; set; }

        public string InstanceName { get; set; }

        public string Description { get; set; }
    }

    public class EcsListDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        private OS OS { get; set; }

        public string Os { get { return OS.GetDescription(); } }

        public string WanIp { get; set; }

        public string InternalIp { get; set; }

        public string DomainName { get; set; }

        private Provider provider { get; set; }

        public string Provider { get { return provider.ToString(); } }

        public string InstanceId { get; set; }

        public string Description { get; set; }

        public string Remark { get; set; }
    }

    public class EcsInfoDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string WanIp { get; set; }

        public string InternalIp { get; set; }

        public OS OS { get; set; }

        public string DomainName { get; set; }

        public Provider Provider { get; set; }

        public string InstanceId { get; set; }

        public string InstanceName { get; set; }

        public Dictionary<string, string> ResourceAuth { get; set; }

        public string Description { get; set; }

        public string Remark { get; set; }

        public Dictionary<string, string> ResourceOption { get; set; }
    }
}