namespace OMTB.Dms.Web.Security
{
    /// <summary>
    /// 当前用户
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 用户Ip
        /// </summary>
        string Ip { get; }

        /// <summary>
        /// 清除当前登录信息
        /// </summary>
        void ClearInfo();

        /// <summary>
        /// 设置当前登录信息
        /// </summary>
        /// <param name="ip"></param>
        void SetInfo(string ip);
    }
}
