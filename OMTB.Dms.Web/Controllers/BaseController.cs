using System.Web.Mvc;
using Microsoft.Practices.Unity;
using OMTB.Component.Data;
using OMTB.Component.Util;
using OMTB.Dms.Service;
using OMTB.Dms.Web.Security;

namespace OMTB.Dms.Web.Controllers
{
    /// <summary>
    /// Controller基础类
    /// </summary>
    public class BaseController : Controller
    {
        [Dependency]
        public IMySession DiMySession { get; set; }

        [Dependency]
        public ICache DiCache { get; set; }

        [Dependency("AppCache")]
        public ICache DiAppCache { get; set; }

        [Dependency]
        public ICurrentUser CurrentUser { get; set; }
        
        [Dependency]
        public LogService DiLogService { get; set; }
        
        [Dependency]
        public ConfigService DiConfigService { get; set; }

        private IUnitOfWork _unitOfWork;

        private IUnitOfWork _readUnitOfWork;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return this._unitOfWork ?? (this._unitOfWork = UnitOfWorkFactory.GetUnitOfWork(UnitOfWorkType.ReadWrite));
            }
        }

        public IUnitOfWork ReadUnitOfWork
        {
            get
            {
                return this._readUnitOfWork ?? (this._readUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(UnitOfWorkType.ReadOnly));
            }
        }

        /// <summary>
        /// 无权访问界面
        /// </summary>
        /// <returns></returns>
        public ActionResult NoFunc()
        {
            ViewBag.CurentUsrIp = Session["CU-Ip"];
            return View();
        }
    }
}