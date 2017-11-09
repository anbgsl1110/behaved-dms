using System.Collections.Generic;

namespace OMTB.Dms.Dto
{
    /// <summary>
    /// 需要参数转换解组的请求对象
    /// </summary>
    public class DescriptionRequest
    {
        private Dictionary<string, string> _queryParameters = new Dictionary<string, string>();

        public string ActionName;
        
        public Dictionary<string, string> QueryParameters
        {
            get
            {
                return this._queryParameters;
            }
            set
            {
                this._queryParameters = value;
            }
        }

        public DescriptionRequest(string actionName)
        {
            this.ActionName = actionName;
        }
    }
}