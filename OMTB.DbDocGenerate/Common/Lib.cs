using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DbDocGenerate.Common
{
    public class Lib
    {
        public static string RequestA(string key)
        {
            string s = System.Web.HttpContext.Current.Request[key];
            if (s == null)
            {
                return null;
            }
            else
            {
                return DoSql(s.ToString().Trim());
            }
        }

        #region 字符串分解
        public static string StringSplit(string strings, int index, string Separ)
        {
            string[] s = splitstr(strings, Separ);
            return s[index];
        }
        /// <summary>
        /// 字符串分函数
        /// </summary>
        /// <param name="str">要分解的字符串</param>
        /// <param name="splitstr">分割符,可以为string类型</param>
        /// <returns>字符数组</returns>
        public static string[] splitstr(string str, string splitstr)
        {
            if (splitstr != "")
            {
                System.Collections.ArrayList c = new System.Collections.ArrayList();
                while (true)
                {
                    int thissplitindex = str.IndexOf(splitstr);
                    if (thissplitindex >= 0)
                    {
                        c.Add(str.Substring(0, thissplitindex));
                        str = str.Substring(thissplitindex + splitstr.Length);
                    }
                    else
                    {
                        c.Add(str);
                        break;
                    }
                }
                string[] d = new string[c.Count];
                for (int i = 0; i < c.Count; i++)
                {
                    d[i] = c[i].ToString();
                }
                return d;
            }
            else
            {
                return new string[] { str };
            }
        }
        /// <summary>
        /// 更新数组第 index 维的值
        /// </summary>
        /// <param name="str">||aa||dd||cc</param>
        /// <param name="index">2</param>
        /// <param name="separ">||</param>
        /// <param name="newstr">bb</param>
        public static string updateSplitstr(string strs, int index, string separ, string newstr)
        {
            string str = strs;
            string[] s = splitstr(str, separ);
            s[index] = newstr;
            str = "";
            for (int i = 0; i < s.Length; i++)
            { str += s[i] + separ; }
            str = str.Substring(0, str.Length - separ.Length);
            return str;
        }
        #endregion
        // 防SQL 注入
        public static string DoSql(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            else
            {
                return str.Replace("'", "''");
            }
        }

        #region Make SQL语句
        private static string FieldNeedQuote(object value, bool hasUnicode = false)
        {
            string ret = null;
            if (value == null) { return " null "; }

            switch (value.GetType().ToString())
            {
                case "System.Int32":
                case "System.Double":
                case "System.Decimal":
                    ret = value.ToString();
                    break;
                case "System.Boolean":
                    if (Convert.ToBoolean(value))
                        ret = "1";
                    else
                        ret = "0";
                    break;
                case "System.DateTime":
                    ret = "'" + Convert.ToString(DateTime.MinValue == Convert.ToDateTime(value) ? System.Data.SqlTypes.SqlDateTime.MinValue : value) + "'";
                    break;
                //case "System.String":
                default:
                    // 防SQL注入
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        ret = "''";
                    }
                    else
                    {
                        if (hasUnicode) { ret = "N"; }

                        if (value.ToString().Replace("''", "").IndexOf('\'') >= 0)
                        {

                            ret += "'" + value.ToString().Replace("'", "''") + "'";
                        }
                        else
                        {
                            ret += "'" + value + "'";
                        }
                    }
                    break;
            }
            return ret;
        }
        public static string MakeInsertSql(string tablename, Hashtable ht)
        {
            string sql = null;
            string fileds = null;
            string values = null;
            foreach (DictionaryEntry de in ht)
            {
                fileds += de.Key.ToString() + ",";
                values += FieldNeedQuote(de.Value) + ",";
            }
            sql += "insert into " + tablename + "(";
            sql += fileds.TrimEnd(',');
            sql += ") values(";
            sql += values.TrimEnd(',');
            sql += ") ";
            return sql;
        }
        public static string MakeUpdateSql(string tablename, Hashtable ht, string where)
        {
            string sql = null;
            sql = "update " + tablename + " set ";
            foreach (DictionaryEntry de in ht)
            {
                sql += de.Key + "=" + FieldNeedQuote(de.Value) + ",";
            }
            sql = sql.TrimEnd(',');
            sql += " where " + where;
            return sql;
        }

        public static string MakeInsertSql(string tablename, IDictionary dict, bool hasUnicode = false)
        {
            string sql = null;
            string fileds = null;
            string values = null;
            foreach (DictionaryEntry de in dict)
            {
                fileds += de.Key.ToString() + ",";
                values += FieldNeedQuote(de.Value, hasUnicode) + ",";
            }
            sql += "insert into " + tablename + "(";
            sql += fileds.TrimEnd(',');
            sql += ") values(";
            sql += values.TrimEnd(',');
            sql += ") ";
            return sql;
        }
        public static string MakeUpdateSql(string tablename, IDictionary dict, string where, bool hasUnicode = false)
        {
            string sql = null;
            sql = "update " + tablename + " set ";
            foreach (DictionaryEntry de in dict)
            {
                sql += de.Key + "=" + FieldNeedQuote(de.Value, hasUnicode) + ",";
            }
            sql = sql.TrimEnd(',');
            sql += " where " + where;
            return sql;
        }
        #endregion

        #region 实体反射
        private static string FieldNeedQuote(Type type, object value, bool hasUnicode = false)
        {
            string ret = null;
            if (value == null) { return " null "; }
            switch (type.FullName)
            {
                case "System.Int32":
                case "System.Double":
                case "System.Decimal":
                    ret = value.ToString();
                    break;
                case "System.Boolean":
                    if (Convert.ToBoolean(value))
                        ret = "1";
                    else
                        ret = "0";
                    break;
                case "System.DateTime":
                    ret = "'" + Convert.ToString(DateTime.MinValue == Convert.ToDateTime(value) ? System.Data.SqlTypes.SqlDateTime.MinValue : value) + "'";
                    break;
                //case "System.String":
                default:
                    // 防SQL注入
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        ret = "''";
                    }
                    else
                    {
                        if (hasUnicode) { ret = "N"; }

                        if (value.ToString().Replace("''", "").IndexOf('\'') >= 0)
                        {

                            ret += "'" + value.ToString().Replace("'", "''") + "'";
                        }
                        else
                        {
                            ret += "'" + value + "'";
                        }
                    }
                    break;
            }
            return ret;
        }

        public static string MakeInsertSql(string table, object entity, bool hasUnicode = false)
        {
            Type _entity = entity.GetType();

            string sql = null;
            string fileds = null;
            string values = null;

            foreach (var pi in _entity.GetProperties())
            {
                if (pi.Name != "ID" && pi.CanRead && pi.CanWrite)
                {
                    fileds += pi.Name + ",";
                    values += FieldNeedQuote(pi.GetValue(entity, null), hasUnicode) + ",";
                }
            }

            sql += "insert into " + table + "(";
            sql += fileds.TrimEnd(',');
            sql += ") values(";
            sql += values.TrimEnd(',');
            sql += ") ";
            return sql;
        }

        public static string MakeUpdateSql(string table, object entity, string where = null, bool hasUnicode = false)
        {
            Type _entity = entity.GetType();
            string sql = null;
            int id = 0;
            sql = "update " + table + " set ";
            foreach (var pi in _entity.GetProperties())
            {
                if (pi.Name == "ID")
                { id = Convert.ToInt32(pi.GetValue(entity, null)); }
                else if (pi.CanRead && pi.CanWrite)
                { sql += pi.Name + "=" + FieldNeedQuote(pi.GetValue(entity, null), hasUnicode) + ","; }
            }

            sql = sql.TrimEnd(',');

            if (string.IsNullOrEmpty(where)) { sql += " where ID=" + id; }
            else { sql += " where " + where; }

            return sql;
        }

        #endregion

        public static string Show_TF(object tf)
        {
            return Show_TF(Utils.Utils.StrToBool(tf));
        }
        public static string Show_TF(bool tf)
        {
            if (tf)
            {
                return "<font color=#009900>√</font>";
            }
            else
            {
                return "<font color=#ff0000>×</font>";
            }
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="Str">要处理字符串</param>
        /// <param name="StrLen">截取长度</param>
        /// <returns></returns>
        public static string CutStr(string Str, int StrLen, string end)
        {
            if (string.IsNullOrEmpty(Str)) return "";
            Str = Utils.Utils.RemoveHtml(Str).Replace("&nbsp;", " ").Replace("  ", " ");
            if (Str != null && Convert.IsDBNull(StrLen) == false)
            {
                int t = 0;
                for (int i = 0; i <= StrLen; i++)
                {
                    if (t >= StrLen)
                    {
                        Str = Str.Substring(0, i) + end;
                        break;
                    }
                    else
                    {
                        if (i >= Str.Length)
                        {
                            Str = Str.Substring(0, i);
                            break;
                        }
                    }
                    int c = Math.Abs((int)Str[i]);
                    if (c > 255)
                    {
                        t += 2;
                    }
                    else
                    {
                        t++;
                    }
                }
            }
            else
            {
                Str = null;
            }
            return Str;
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="Str">要处理字符串</param>
        /// <param name="StrLen">截取长度</param>
        /// <returns>字符串...</returns>
        public static string CutStr(string Str, int StrLen)
        {
            return CutStr(Str, StrLen, "");
        }

        /*public static string md5(string str, int code)
        {
            if (code == 16) //16位MD5加密（取32位加密的9~25字符） 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            else//32位加密 
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }
        }*/

        /// <summary>
        /// 返回格式 2天5小时49分30秒 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetLastTime(DateTime dt)
        {
            TimeSpan ts = new TimeSpan();
            ts = dt - DateTime.Now;
            string ret = null;

            if (dt < DateTime.Now) return "已经结束";

            if (ts.Days > 0)
            {
                ret += ts.Days + "天";
            }
            if (ts.Hours > 0)
            {
                ret += ts.Hours + "小时";
            }
            if (ts.Minutes > 0)
            {
                ret += ts.Minutes + "分";
            }
            if (ts.Seconds > 0)
            {
                ret += ts.Seconds + "秒";
            }
            return ret;
        }

        public static string GetUseTime(int second)
        {
            TimeSpan ts = new TimeSpan(0, 0, second);

            string ret = "";

            if (ts.Days > 0)
            {
                ret += ts.Days + "天";
            }
            if (ts.Hours > 0)
            {
                ret += ts.Hours + "小时";
            }
            if (ts.Minutes > 0)
            {
                ret += ts.Minutes + "分";
            }
            if (ts.Seconds > 0)
            {
                ret += ts.Seconds + "秒";
            }
            return ret;
        }

        public static string GetUseTime24Second(int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, seconds);

            string ret = "";

            if (ts.Hours >= 0)
            {
                ret += (ts.Days * 24 + ts.Hours).ToString("00") + ":";
            }
            if (ts.Minutes >= 0)
            {
                ret += ts.Minutes.ToString("00") + ":";
            }
            if (ts.Seconds >= 0)
            {
                ret += ts.Seconds.ToString("00");
            }
            return ret;
        }


        public static int GetDiffDate(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = new TimeSpan();
            ts = dt1 - dt2;
            return ts.Days;
        }

        public static string GetDomainPreName(string domainname)
        {
            if (string.IsNullOrEmpty(domainname))
            { return ""; }
            if (domainname.IndexOf('.') >= 0)
            {
                return domainname.Substring(0, domainname.IndexOf('.'));
            }
            else
            { return ""; }
        }

        public static string GetBeginEndString(string s, string b, string e, int num)
        {
            string tmpstr = s;

            int pos = 0;
            for (int i = 0; i < num; i++)
            {
                if (tmpstr.IndexOf(b) == -1) return "";
                pos = tmpstr.IndexOf(b) + b.Length;
                tmpstr = tmpstr.Substring(pos);
            }
            tmpstr = tmpstr.Substring(0, tmpstr.IndexOf(e));
            return tmpstr;
        }

        /// <summary>
        /// 取电话的分机号
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string GetPhoneExt(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.IndexOf('-') == -1)
            { return ""; }
            string ext = phone.Substring(phone.LastIndexOf('-') + 1);
            if (ext.Length <= 5)
            { return ext; }
            else
            { return ""; }
        }
        /// <summary>
        /// 取电话的主体部分, 不含分机号
        /// </summary>
        /// <returns></returns>
        public static string GetPhoneBody(string phone)
        {
            string ext = GetPhoneExt(phone);
            if (string.IsNullOrEmpty(ext)) return phone;

            return phone.Substring(0, phone.LastIndexOf('-'));
        }

        public static bool IsEnDomain(string domainname)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(domainname, @"^[a-z|A-Z|0-9|\-|\.]*$");
        }

        // 在unicode 字符串中，中文的范围是在4E00..9FFF:CJK Unified Ideographs。 
        // 通过对字符的unicode编码进行判断来确定字符是否为中文。 
        protected bool IsChineseLetter(string input, int index)
        {
            int code = 0;
            int chfrom = Convert.ToInt32("4e00", 16); //范围（0x4e00～0x9fff）转换成int（chfrom～chend） 
            int chend = Convert.ToInt32("9fff", 16);
            if (input != "")
            {
                code = Char.ConvertToUtf32(input, index); //获得字符串input中指定索引index处字符unicode编码 

                if (code >= chfrom && code <= chend)
                {
                    return true; //当code在中文范围内返回true 
                }
                else
                {
                    return false; //当code不在中文范围内返回false
                }
            }
            return false;
        }
        #region 判断指定的字符串是否包含 中文字符
        /// <summary>
        /// 判断指定的字符串是否包含 中文字符
        /// </summary>
        /// <param name="str">要判断的字符串</param>
        /// <returns></returns>
        public static bool IsIncludeChina(string str)
        {
            bool BoolValue = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(str.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    return BoolValue = true;
                }
            }
            return BoolValue;
        }
        #endregion

        #region 域名的判断
        /// <summary>
        /// 判断指定的定符串是否有效域名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static bool IsDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain) || domain.IndexOf('.') == -1 || domain.IndexOf(' ') != -1)
                return false;

            string suffix = domain.Substring(domain.IndexOf('.'));
            //获取域名主体部分
            string strDomain = domain.Substring(0, domain.IndexOf('.'));
            //判断是否以-开头，以-结尾
            if (strDomain[0] == '-' || strDomain[strDomain.Length - 1] == '-')
            {
                return false;
            }
            //判断域名是否同时包含两个-
            if (System.Text.RegularExpressions.Regex.IsMatch(domain, @"^\w--\w$"))
            {
                return false;
            }
            //域名长度的判断，国际域名长度不能超过67个(含.com.cn后缀）。国内域名不能超过26个字符
            if (IsIncludeChina(domain))
            {
                if (domain.Length > 26)
                    return false;
            }
            else
            {
                if (domain.Length > 67)
                    return false;
            }
            //域名可以是字母，数定，-中文字符
            if (!System.Text.RegularExpressions.Regex.IsMatch(strDomain.ToLower(), @"^([0-9]*[a-z]*[\u4e00-\u9fa5]*)*-?([0-9]*[a-z]*[\u4e00-\u9fa5]*)*$"))
                return false;
            switch (suffix)
            {
                case ".cn":
                #region
                case ".com":
                case ".net":
                case ".org":
                case ".gov":
                case ".ac":
                case ".bj":
                case ".sh":
                case ".tj":
                case ".cq":
                case ".he":
                case ".sx":
                case ".nm":
                case ".ln":
                case ".jl":
                case ".hl":
                case ".js":
                case ".zj":
                case ".ah":
                case ".fj":
                case ".jx":
                case ".sd":
                case ".ha":
                case ".hb":
                case ".hn":
                case ".gd":
                case ".gx":
                case ".hi":
                case ".sc":
                case ".gz":
                case ".yn":
                case ".xz":
                case ".sn":
                case ".gs":
                case ".qh":
                case ".nx":
                case ".xj":
                case ".tw":
                #endregion

                #region
                case ".com.cn":
                case ".net.cn":
                case ".org.cn":
                case ".gov.cn":
                case ".ac.cn":
                case ".bj.cn":
                case ".sh.cn":
                case ".tj.cn":
                case ".cq.cn":
                case ".he.cn":
                case ".sx.cn":
                case ".nm.cn":
                case ".ln.cn":
                case ".jl.cn":
                case ".hl.cn":
                case ".js.cn":
                case ".zj.cn":
                case ".ah.cn":
                case ".fj.cn":
                case ".jx.cn":
                case ".sd.cn":
                case ".ha.cn":
                case ".hb.cn":
                case ".hn.cn":
                case ".gd.cn":
                case ".gx.cn":
                case ".hi.cn":
                case ".sc.cn":
                case ".gz.cn":
                case ".yn.cn":
                case ".xz.cn":
                case ".sn.cn":
                case ".gs.cn":
                case ".qh.cn":
                case ".nx.cn":
                case ".xj.cn":
                case ".tw.cn":
                case ".hk.cn":
                case ".mo.cn":
                #endregion
                case ".中国":
                case ".网络":
                case ".公司":
                    return true;
                default:
                    return false;
            }
        }
        #endregion
        public bool IsChina(string CString)
        {
            bool BoolValue = false;
            for (int i = 0; i < CString.Length; i++)
            {
                if (Convert.ToInt32(Convert.ToChar(CString.Substring(i, 1))) < Convert.ToInt32(Convert.ToChar(128)))
                {
                    BoolValue = false;
                }
                else
                {
                    return BoolValue = true;
                }
            }
            return BoolValue;
        }

        /// <summary> 
        /// 判断句子中是否含有中文 
        /// </summary> 
        /// <param >字符串</param> 
        public bool WordsIScn(string words)
        {
            string TmmP;
            for (int i = 0; i < words.Length; i++)
            {
                TmmP = words.Substring(i, 1);
                byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(TmmP);
                if (sarr.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }

        //for (int i=0; i<s.length; i++) 
        //{ 
        //  Regex rx = new Regex("^[\u4e00-\u9fa5]$"); 
        //  if (rx.IsMatch(s[i])) 
        //      是 
        //  else 
        //      否 
        //} 
        //正解！ 
        //\u4e00-\u9fa5 汉字的范围。 
        //^[\u4e00-\u9fa5]$ 汉字的范围的正则 
        //方法五： 
        //unicodeencoding unicodeencoding = new unicodeencoding(); 
        //byte [] unicodebytearray = unicodeencoding.getbytes( inputstring ); 
        //for( int i = 0; i < unicodebytearray.length; i++ ) 
        //{ 
        //i++; 
        //  //如果是中文字符那么高位不为0 
        //if ( unicodebytearray[i] != 0 ) 
        //{ 
        //} 
        //…… 
        //方法六： 
        /// <summary> 
        /// 给定一个字符串，判断其是否只包含有汉字 
        /// </summary> 
        /// <param name="testStr"></param> 
        /// <returns></returns> 
        public bool IsOnlyContainsChinese(string testStr)
        {
            char[] words = testStr.ToCharArray();
            foreach (char word in words)
            {
                if (IsGBCode(word.ToString()) || IsGBKCode(word.ToString())) // it is a GB2312 or GBK chinese word 
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary> 
        /// 判断一个word是否为GB2312编码的汉字 
        /// </summary> 
        /// <param name="word"></param> 
        /// <returns></returns> 
        private bool IsGBCode(string word)
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(word);
            if (bytes.Length <= 1) // if there is only one byte, it is ASCII code or other code 
            {
                return false;
            }
            else
            {
                byte byte1 = bytes[0];
                byte byte2 = bytes[1];
                if (byte1 >= 176 && byte1 <= 247 && byte2 >= 160 && byte2 <= 254) //判断是否是GB2312 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary> 
        /// 判断一个word是否为GBK编码的汉字 
        /// </summary> 
        /// <param name="word"></param> 
        /// <returns></returns> 
        private bool IsGBKCode(string word)
        {
            byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(word.ToString());
            if (bytes.Length <= 1) // if there is only one byte, it is ASCII code 
            {
                return false;
            }
            else
            {
                byte byte1 = bytes[0];
                byte byte2 = bytes[1];
                if (byte1 >= 129 && byte1 <= 254 && byte2 >= 64 && byte2 <= 254) //判断是否是GBK编码 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary> 
        /// 判断一个word是否为Big5编码的汉字 
        /// </summary> 
        /// <param name="word"></param> 
        /// <returns></returns> 
        private bool IsBig5Code(string word)
        {
            byte[] bytes = Encoding.GetEncoding("Big5").GetBytes(word.ToString());
            if (bytes.Length <= 1) // if there is only one byte, it is ASCII code 
            {
                return false;
            }
            else
            {
                byte byte1 = bytes[0];
                byte byte2 = bytes[1];
                if ((byte1 >= 129 && byte1 <= 254) && ((byte2 >= 64 && byte2 <= 126) || (byte2 >= 161 && byte2 <= 254))) //判断是否是Big5编码 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /*public static string Traditional2Simplified(string str)
        { //繁体转简体 
            return (Microsoft.VisualBasic.Strings.StrConv(str, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0));
        }
        public static string Simplified2Traditional(string str)
        { //简体转繁体 
            return (Microsoft.VisualBasic.Strings.StrConv(str as String, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0));
        }*/

        /// <summary>
        /// 域名后缀
        /// </summary>
        /// <returns></returns>
        public static string GetSuffix(string domain)
        {
            if (string.IsNullOrEmpty(domain)) return "";
            int pos = domain.IndexOf('.');
            if (pos < 1) return "";

            return domain.Substring(pos);
        }

        public static bool IsChCnnicDomainSuffix(string domain)
        {
            string suffix = GetSuffix(domain);

            if (suffix == ".中国" || suffix == ".公司" || suffix == ".网络")
                return true;
            else
                return false;
        }

        /// <summary>
        /// 1970 到现在的毫秒数
        /// </summary>
        /// <returns></returns>
        public static string GetTotalMilliseconds()
        {
            TimeSpan ts = new TimeSpan();
            ts = DateTime.Now - Convert.ToDateTime("1970-1-1 8:00:00");
            return ts.TotalMilliseconds.ToString().Replace(".", "");
        }

        /// <summary>
        /// 页面分页显示功能
        /// </summary>
        /// <param name="Parameters">参数串(a=1&amp;b=2...)</param>
        /// <param name="RecordCount">记录总数</param>
        /// <param name="PageSize"></param>
        /// <param name="CurrentPage">当前页号</param>
        /// <returns></returns>
        public static string Paging(string Parameters, int RecordCount, int PageSize, int currentPage, int NumericButtonCount)
        {
            int CurrentPage = currentPage;
            int PageCount = (int)Math.Ceiling(Convert.ToDouble(RecordCount) / PageSize);
            string str = "";
            //if (RecordCount <= PageSize) return "";

            if (!string.IsNullOrEmpty(Parameters)) Parameters = Parameters.Trim().Trim('&');
            if (!string.IsNullOrEmpty(Parameters)) Parameters += "&";
            if (PageCount < 1) PageCount = 1;
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > PageCount) CurrentPage = PageCount;

            str = "<div>";

            str += "<div style=\"float:left;\">";
            str += "共 " + RecordCount + " 条记录 页次：" + CurrentPage + "/" + PageCount + "页 ";
            str += PageSize + "条/页 ";
            str += "</div>";

            str += "<div style=\"float:right;\">";
            if (CurrentPage == 1)
            {
                str += "<span style=\"color:#999;\">首页 上页</span> ";
            }
            else
            {
                str += "<a href='?" + Parameters + "page=1'  class=\"link\">首页</a> ";
                str += "<a href='?" + Parameters + "page=" + (CurrentPage - 1) + "'  class=\"link\">上页</a> "; ;
            }

            int NumberSize = NumericButtonCount;
            int PageNumber = (CurrentPage - 1) / NumberSize;

            if (PageNumber * NumberSize > 0)
                str += "<a href='?" + Parameters + "page=" + PageNumber * NumberSize + "'>…</a> ";
            int i;
            for (i = PageNumber * NumberSize + 1; i <= (PageNumber + 1) * NumberSize; i++)
            {
                if (i == CurrentPage)
                    str += "<span style=\"color:#FF0000;font-weight:bold;\">[" + i + "]</span> ";
                else
                    str += "<a href='?" + Parameters + "page=" + i + "'>[" + i + "]</a> ";
                if (i == PageCount) break;
            }
            if ((PageNumber + 1) * NumberSize < PageCount) str += "<a href='?" + Parameters + "page=" + i + "'>…</a>  ";

            if (CurrentPage == PageCount)
            {
                str += "<span style=\"color:#999;\">下页 尾页</span> ";
            }
            else
            {
                str += "<a href='?" + Parameters + "page=" + (CurrentPage + 1) + "'  class=\"link\">下页</a> ";
                str += "<a href='?" + Parameters + "page=" + PageCount + "'  class=\"link\">尾页</a>  ";
            }
            str += "</div>";
            str += "</div>";
            return str;
        }

        /// <summary>
        /// 页面分页显示功能
        /// </summary>
        /// <param name="Parameters">参数串(a=1&amp;b=2...)</param>
        /// <param name="NumericButtonCount">当前页参数的名称</param>
        /// <param name="RecordCount">记录总数</param>
        /// <param name="PageSize"></param>
        /// <param name="CurrentPage">当前页号</param>
        /// <returns></returns>
        public static string Paging(string Parameters, string PageParamName, int RecordCount, int PageSize, int currentPage, int NumericButtonCount)
        {
            int CurrentPage = currentPage;
            int PageCount = (int)Math.Ceiling(Convert.ToDouble(RecordCount) / PageSize);
            string str = "";
            //if (RecordCount <= PageSize) return "";
            if (!string.IsNullOrEmpty(Parameters)) Parameters = Parameters.Trim().Trim('&');
            if (!string.IsNullOrEmpty(Parameters)) Parameters += "&";
            if (CurrentPage < 1) CurrentPage = 1;
            if (PageCount < 1) PageCount = 1;
            if (CurrentPage > PageCount) CurrentPage = PageCount;

            str = "<div>";

            str += "<div style=\"float:left;\">";
            str += "共 " + RecordCount + " 条记录 页次：" + CurrentPage + "/" + PageCount + "页 ";
            str += PageSize + "条/页 ";
            str += "</div>";

            str += "<div style=\"float:right;\">";
            if (CurrentPage == 1)
            {
                str += "<span style=\"color:#999;\">首页 上页</span> ";
            }
            else
            {
                str += "<a href='?" + Parameters + PageParamName + "=1'  class=\"link\" data-page=\"1\" >首页</a> ";
                str += "<a href='?" + Parameters + PageParamName + "=" + (CurrentPage - 1) + "' data-page=\"" + (CurrentPage - 1) + "\"  class=\"link\">上页</a> "; ;
            }

            int NumberSize = NumericButtonCount;
            int PageNumber = (CurrentPage - 1) / NumberSize;

            if (PageNumber * NumberSize > 0)
                str += "<a href='?" + Parameters + PageParamName + "=" + PageNumber * NumberSize + "' data-page=\"" + PageNumber * NumberSize + "\">…</a> ";
            int i;
            for (i = PageNumber * NumberSize + 1; i <= (PageNumber + 1) * NumberSize; i++)
            {
                if (i == CurrentPage)
                    str += "<span style=\"color:#FF0000;font-weight:bold;\">[" + i + "]</span> ";
                else
                    str += "<a href='?" + Parameters + PageParamName + "=" + i + "' data-page=\"" + i + "\">[" + i + "]</a> ";
                if (i == PageCount) break;
            }
            if ((PageNumber + 1) * NumberSize < PageCount) str += "<a href='?" + Parameters + PageParamName + "=" + i + "'>…</a>  ";

            if (CurrentPage == PageCount)
            {
                str += "<span style=\"color:#999;\">下页 尾页</span> ";
            }
            else
            {
                str += "<a href='?" + Parameters + PageParamName + "=" + +(CurrentPage + 1) + "' data-page=\"" + (CurrentPage + 1) + "\"  class=\"link\">下页</a> ";
                str += "<a href='?" + Parameters + PageParamName + "=" + +PageCount + "' data-page=\"" + PageCount + "\"  class=\"link\">尾页</a>  ";
            }
            str += "</div>";
            str += "</div>";
            return str;
        }


        public static string LoseHtml(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return System.Text.RegularExpressions.Regex.Replace(str, @"<[^>]+>|</[^>]+>", "");
        }
        public static string ToHTML(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            return html.Replace("\r\n", "<br />");
        }

        public static Hashtable DeserializeIni(string filename)
        {
            Hashtable ht = new Hashtable();
            try
            {
                StreamReader fs = new StreamReader(HttpContext.Current.Server.MapPath(filename));
                try
                {
                    string s = fs.ReadToEnd();
                    s += "\n";
                    String p = @"(?<key>[\s\S]*?)=(?<value>[\s\S]*?)[\n]";
                    Regex reg = new Regex(p, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    MatchCollection ms = reg.Matches(s);

                    //HttpContext.Current.Response.Write(ms.Count);
                    foreach (Match m in ms)
                    {
                        ht.Add(m.Groups["key"].Value.Trim(), m.Groups["value"].Value.Trim());

                        //HttpContext.Current.Response.Write(m.Groups["key"].Value.Trim() + "==>" + m.Groups["value"].Value);
                    }
                }
                catch (Exception e)
                {
                    HttpContext.Current.Response.Write("Failed: " + e.Message);
                    HttpContext.Current.Response.End();
                }
                finally
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
                HttpContext.Current.Response.End();
            }
            return ht;
        }

        /// <summary>
        /// INI 格式字符串转 HashTable
        /// </summary>
        /// <param name="str"></param>
        public static Hashtable DeserializeIniStr(string str)
        {
            Hashtable ht = new Hashtable();

            try
            {
                string s = str;
                s += "\n";
                String p = @"(?<key>[\s\S]*?)=(?<value>[\s\S]*?)[\n]";
                Regex reg = new Regex(p, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                MatchCollection ms = reg.Matches(s);

                //HttpContext.Current.Response.Write(ms.Count);
                foreach (Match m in ms)
                {
                    ht.Add(m.Groups["key"].Value.Trim(), m.Groups["value"].Value.Trim());

                    //HttpContext.Current.Response.Write(m.Groups["key"].Value.Trim() + "==>" + m.Groups["value"].Value);
                }
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write("Failed: " + e.Message);
                HttpContext.Current.Response.End();
            }

            return ht;
        }

        public static int GetWeek(DateTime dt)
        {
            string firstDateText = dt.Year.ToString() + "年1月1日";
            DateTime firstDay = Convert.ToDateTime(firstDateText);
            int theday;

            if (firstDay.DayOfWeek == DayOfWeek.Sunday || firstDay.DayOfWeek == DayOfWeek.Monday)
            {
                theday = 0;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Tuesday)
            {
                theday = 1;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Wednesday)
            {
                theday = 2;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Thursday)
            {
                theday = 3;
            }
            else if (firstDay.DayOfWeek == DayOfWeek.Friday)
            {
                theday = 4;
            }
            else
            {
                theday = 5;
            }

            int weekNum = (dt.DayOfYear + theday) / 7 + 1;

            return weekNum;
        }

        /// <summary>
        /// 返回 12:01, 昨天 12:01, 前天 12:01， yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetTime(DateTime dt)
        {
            TimeSpan ts = new TimeSpan();
            ts = DateTime.Now.Date - dt.Date;

            // 今天
            if (ts.TotalDays == 0)
            {
                return "今天 " + dt.ToString("HH:mm");
            }
            if (ts.TotalDays == 1)
            {
                return dt.ToString("昨天 " + dt.ToString("HH:mm"));
            }
            if (ts.TotalDays == 2)
            {
                return dt.ToString("前天 " + dt.ToString("HH:mm"));
            }
            return dt.ToString("yyy-MM-dd");
        }

        /// <summary>  
        /// 过滤特殊字符  
        /// </summary>  
        /// <param name="s"></param>  
        /// <returns></returns>  
        public static string String2Json(String s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        public static string Int2IP(UInt32 ipCode)
        {
            byte a = (byte)((ipCode & 0xFF000000) >> 0x18);
            byte b = (byte)((ipCode & 0x00FF0000) >> 0x10);
            byte c = (byte)((ipCode & 0x0000FF00) >> 0x8);
            byte d = (byte)(ipCode & 0x000000FF);
            string ipStr = String.Format("{0}.{1}.{2}.{3}", a, b, c, d);
            return ipStr;
        }

        public static UInt32 IP2Int(string ipStr)
        {
            string[] ip = ipStr.Split('.');
            if (ip.Length != 4) return 0;

            uint ipCode = 0xFFFFFF00 | byte.Parse(ip[3]);
            ipCode = ipCode & 0xFFFF00FF | (uint.Parse(ip[2]) << 0x8);
            ipCode = ipCode & 0xFF00FFFF | (uint.Parse(ip[1]) << 0x10);
            ipCode = ipCode & 0x00FFFFFF | (uint.Parse(ip[0]) << 0x18);
            return ipCode;
        }

        /// <summary>
        /// 获取一个目标的匹配结果
        /// </summary>
        /// <param name="input">要匹配的字符串</param>
        /// <param name="pattern"></param>
        /// <param name="find"></param>
        /// <returns></returns>
        public static Match GetMatch(string input, string pattern, string find)
        {
            string _pattn = Regex.Escape(pattern);
            _pattn = _pattn.Replace(@"\[变量]", @"[\s\S]*?");
            _pattn = Regex.Replace(_pattn, @"((\\r\\n)|(\\ ))+", @"\s*", RegexOptions.Compiled);
            if (Regex.Match(pattern.TrimEnd(), Regex.Escape(find) + "$", RegexOptions.Compiled).Success)
                _pattn = _pattn.Replace(@"\" + find, @"(?<TARGET>[\s\S]+)");
            else
                _pattn = _pattn.Replace(@"\" + find, @"(?<TARGET>[\s\S]+?)");
            Regex r = new Regex(_pattn, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m = r.Match(input);
            return m;
        }

        public static string FormatPattern(string pattern)
        {
            // string _pattn = Regex.Escape(pattern);
            string _pattn = pattern;
            _pattn = _pattn.Replace(@"[变量]", @"[\s\S]*?");
            _pattn = Regex.Replace(_pattn, @"((\r\n)|(\ ))+", @"\s*", RegexOptions.Compiled);
            return _pattn;
        }

        public static Match GetMatch(string input, string pattern)
        {
            // string _pattn = Regex.Escape(pattern);
            string _pattn = pattern;
            _pattn = _pattn.Replace(@"[变量]", @"[\s\S]*?");
            _pattn = Regex.Replace(_pattn, @"((\r\n)|(\ ))+", @"\s*", RegexOptions.Compiled);
            Regex r = new Regex(_pattn, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m = r.Match(input);
            return m;
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;

            result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("#", "");
                string[] ips = result.Split(',');
                foreach (string ip in ips)
                {
                    if (IsIP(ip.Trim()))
                    {
                        result = ip.Trim();
                        break;
                    }
                }
            }
            if (null == result || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty)
            {
                return "0.0.0.0";
            }

            return result;
        }

        public static string GetSquidIP()
        {
            string result = String.Empty;

            string str = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(str))
            {
                string[] ips = str.Split(',');
                if (ips.Length > 1)
                {
                    for (int i = ips.Length - 1; i >= 0; i--)
                    {
                        if (IsIP(ips[i].Trim()))
                        {
                            result = ips[i].Trim();
                            break;
                        }
                    }
                }
            }
            if (null == result || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            if (null == result || result == String.Empty)
            {
                result = System.Web.HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty)
            {
                return "0.0.0.0";
            }

            return result;
        }

        public static bool IsIP(string ip)
        {
            //判断是否为IP
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static DateTime GetSmallDate(DateTime dt)
        {
            DateTime MinDate = Convert.ToDateTime("1970-1-1");
            if (dt < MinDate)
            {
                return MinDate;
            }
            return dt;
        }

        #region 以字段对List分组排序
        /// <summary>
        /// 以字段对List分组排序
        /// </summary>
        /// <param name="list"></param>
        /// <param name="field"></param>
        /// <param name="genArr"></param>
        /// <param name="IsBack">返回时是否按照原来的排序</param>
        /// <returns></returns>
        public static SortedList MapDataToRowBySorted(IList<Dictionary<string, object>> list, string field, bool genArr)
        {
            SortedList rtList = new SortedList();

            for (int i = 0; i < list.Count; i++)
            {
                if (genArr)
                {
                    if (!rtList.ContainsKey(list[i][field]))
                    {
                        rtList[list[i][field]] = new List<Dictionary<string, object>>();
                    }
                    (rtList[list[i][field]] as List<Dictionary<string, object>>).Add(list[i]);
                }
                else
                {
                    rtList[list[i][field]] = list[i];
                }
            }

            return rtList;
        }
        #endregion

        /// <summary>
        /// 实体反射赋值方法 added by xuc 2016.03.03
        /// </summary>
        /// <typeparam name="T">待转换实体类型</typeparam>
        /// <typeparam name="T1">返回实体类型</typeparam>
        /// <param name="obj">待转换实体实例</param>
        /// <returns></returns>
        public static T1 Converter<T, T1>(T obj)
        {
            //创建返回Entity的实体类
            T1 t = Activator.CreateInstance<T1>();

            //反射属性
            PropertyInfo[] retProperties = typeof(T1).GetProperties();
            PropertyInfo[] properties = typeof(T).GetProperties();

            //反射给entity赋值
            foreach (PropertyInfo pi in retProperties)
            {
                if (properties.Count(p => p.Name == pi.Name && p.PropertyType == pi.PropertyType) <= 0)
                    continue;

                var val = pi.GetValue(obj, null);
                pi.SetValue(t, val, null);
            }

            return t;
        }
    }
}