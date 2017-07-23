using Microsoft.Practices.Unity;
using OMTB.Component.Data;
using OMTB.Component.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResourcePwdManage.Controllers
{
    public class BaseController : Controller
    {
        [Dependency]
        public IMySession DiMySession { get; set; }

        [Dependency]
        public ICache DiCache { get; set; }

        [Dependency("AppCache")]
        public ICache DiAppCache { get; set; }

        //public CurrentUser CurrentUser { get; set; }

        private IUnitOfWork unitOfWork;

        private IUnitOfWork readUnitOfWork;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return this.unitOfWork ?? (this.unitOfWork = UnitOfWorkFactory.GetUnitOfWork(UnitOfWorkType.ReadWrite));
            }
        }

        public IUnitOfWork ReadUnitOfWork
        {
            get
            {
                return this.readUnitOfWork ?? (this.readUnitOfWork = UnitOfWorkFactory.GetUnitOfWork(UnitOfWorkType.ReadOnly));
            }
        }

        public BaseController()
        {
            //CurrentUser = CurrentContext.GetCurrentUser();
        }
    }
}