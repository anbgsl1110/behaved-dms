using ResourcePwdManage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Greedy.Dapper;

namespace ResourcePwdManage.Controllers
{
    public class EcsController : BaseController
    {
        //
        // GET: /Ecs/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetEcsList()
        {
            var querySql = new StringBuilder();
            querySql.Append("select * from ecs");
            Dictionary<string, object> param = new Dictionary<string, object>();
            var rs = new List<EcsListDto>();
            using (var conn = ReadUnitOfWork)
            {
                rs = conn.Connection.Query<EcsListDto>(querySql.ToString(), param).ToList();
                querySql.Clear();
                return Json(rs);
            }
        }
    }
}
