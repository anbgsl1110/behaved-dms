using System.Collections.Generic;

namespace OMTB.Dms.Dto
{
    /// <summary>
    /// Ip白名单信息
    /// </summary>
    public class IpWhiteInfo
    {
        /// <summary>
        /// Ip白名单地址集合
        /// </summary>
        public List<string> IpWhiteAddressList { get; set; }
        
        /// <summary>
        /// Ip白名单可用域集合
        /// </summary>
        public List<string> IpWhiteAreaList { get; set; }
    }
}