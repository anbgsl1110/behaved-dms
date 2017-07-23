using System.Collections.Generic;
using System.Linq;

namespace OMTB.Component.Reader
{
    public class SqlReader : IReader
    {
        private readonly Dictionary<string, string> _sqlScriptContext = new Dictionary<string, string>();

        public Dictionary<string, string> SqlScriptContext
        {
            get { return this._sqlScriptContext; }
            set
            {
                Dictionary<string,string> tempDic = value;
                if (_sqlScriptContext.ContainsKey(tempDic.Keys.First()))
                {
                    _sqlScriptContext.Remove(tempDic.Keys.First());
                }
                _sqlScriptContext.Add(tempDic.Keys.First(),tempDic.Values.First());
            }
        }

        public void Read(string fileName, string path)
        {        
            string readResponseString = "";
            var fileContext = System.IO.File.ReadAllText(path);
            if (!string.IsNullOrWhiteSpace(fileContext))
            {
                readResponseString = fileContext.Trim();
            }
            
            SqlScriptContext = new Dictionary<string, string> {{fileName, readResponseString}};
        }
    }
}