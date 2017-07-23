using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ResourcePwdManage.Entity
{
    public static class Common
    {
        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute attribute = (DescriptionAttribute)value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }


    public enum Provider
    {
        阿里云 = 1,
        腾讯云 = 2,
        景安网络 = 3,
    }

    public enum OS
    {
        [DescriptionAttribute("Windows 2008")]
        Win2008 = 1,

        [DescriptionAttribute("Windows 2003")]
        Win2003 = 2,

        [DescriptionAttribute("CentOS 6.5")]
        CentOS65 = 3,
    }

    public class ResourceAuth
    {
        public long Id { get; set; }

        public short ResourceType { get; set; }

        public int ResourceId { get; set; }

        public string AuthKey { get; set; }

        public string AuthPwd { get; set; }

        public string Description { get; set; }
    }

    public class ResourceOption
    {
        public long Id { get; set; }

        public short ResourceType { get; set; }

        public int ResourceId { get; set; }

        public string OptionKey { get; set; }

        public string OptionValue { get; set; }

        public string Description { get; set; }
    }

}