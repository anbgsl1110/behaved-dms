namespace OMTB.Dms.Dto
{
    /// <summary>
    /// RdsApi公共请求参数
    /// </summary>
    public class RdsApiShareRequestParameter
    {
        /// <summary>
        /// 返回值的类型，支持JSON与XML。默认为XML
        /// </summary>
        public string Format { get; set; }
        
        /// <summary>
        /// API版本号，为日期形式：YYYY-MM-DD，本版本对应为2014-08-15
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// 阿里云颁发给用户的访问服务所用的密钥ID
        /// </summary>
        public string AccessKeyId { get; set; }
        
        /// <summary>
        /// 签名结果串
        /// </summary>
        public string Signature { get; set; }
        
        /// <summary>
        /// 签名方式，目前支持HMAC-SHA1
        /// </summary>
        public string SignatureMethod { get; set; }
        
        /// <summary>
        /// 请求的时间戳。
        /// 日期格式按照ISO8601标准表示，并需要使用UTC时间。
        /// 格式为：YYYY-MM-DDThh:mm:ssZ;
        /// 例如，2013-08-15T12:00:00Z（为北京时间2013年8月15日20点0分0秒）
        /// </summary>
        public string Timestamp { get; set; }
        
        /// <summary>
        /// 签名算法版本，目前版本是1.0
        /// </summary>
        public string SignatureVersion { get; set; }
        
        /// <summary>
        /// 唯一随机数，用于防止网络重放攻击。
        /// 用户在不同请求间要使用不同的随机数值
        /// </summary>
        public string SignatureNonce { get; set; }
    }
}