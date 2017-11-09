using OMTB.Component.Util;

namespace OMTB.Dms.Web.Security
{
    public class CurrentUser : ICurrentUser
    {
        IMySession session;


        public CurrentUser(IMySession mySession)
        {
            session = mySession;
        }

        public string Ip {
            get { return session.Get<string>("CU-Ip"); }
        }

        public void ClearInfo()
        {
            session["CU-Ip"] = null;
        }

        public void SetInfo(string ip)
        {
            session.Set("CU-Ip", ip);
        }
    }
}