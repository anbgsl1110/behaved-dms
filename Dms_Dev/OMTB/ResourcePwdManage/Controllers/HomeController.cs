using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ResourcePwdManage.Models;
using ResourcePwdManage.Helpers;
using System.Threading.Tasks;
using System.Threading;
using OMTB.Component.Data;
using Microsoft.Practices.Unity;

namespace ResourcePwdManage.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        static IList<string> pwdCharList = new List<string> { "1","2","3","4","5","6","7","8","9","0",
            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","d","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AboutPassword()
        {
            return View();
        }

        public ActionResult GetRandomPassword(int pwdLength)
        {
            var connPrefix = new StringBuilder();
            if (pwdLength > 0)
            {
                Random random = new Random();
                for (int r = 0; r < pwdLength; r++)
                {
                    connPrefix.Append(pwdCharList[random.Next(pwdCharList.Count)]);
                }

            }
            return Json(new { data = connPrefix.ToString() });
        }


        public ActionResult GetMd5String(string sourseStr)
        {
            var result = new MD5Password();
            if (!string.IsNullOrEmpty(sourseStr))
            {
                var result16 = sourseStr.Md5To16Byte();
                var result32 = sourseStr.Md5To32Byte();
                result = new MD5Password
                {
                    data16Lower = result16.ToLower(),
                    data16Upper = result16.ToUpper(),
                    data32Lower = result32.ToLower(),
                    data32Upper = result32.ToUpper()
                };
            }
            return Json(result);
        }

    }
}
