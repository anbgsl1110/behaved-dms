using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ResourcePwdManage.Models
{
    public class MD5Password
    {
        public string data16Lower { get; set; }
        public string data16Upper { get; set; }
        public string data32Lower { get; set; }
        public string data32Upper { get; set; }
    }
}