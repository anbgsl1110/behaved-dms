using System.Collections.Specialized;
using System.Configuration;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;
using OMTB.Component.Data;
using OMTB.Component.Util;
using OMTB.Dms.IService;
using OMTB.Dms.Service;
using OMTB.Dms.Web.Security;

namespace OMTB.Dms.Web
{
    public sealed class Bootstrapper
    {
        private static Bootstrapper _strapper = null;

        public static Bootstrapper Instance
        {
            get
            {
                if (_strapper == null)
                {
                    _strapper = new Bootstrapper();
                }
                return _strapper;
            }
        }

        private IUnityContainer _container = null;

        public IUnityContainer UnityContainer
        {
            get
            {
                return this._container;
            }
        }

        public void Initialise()
        {
            BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(this._container));
        }

        public void BuildUnityContainer()
        {
            /*
             * 
             * Unity默认情况下会自动帮我们维护好这些对象的生命周
             * 
             * TransientLifetimeManager，瞬态生命周期，默认情况下，在使用RegisterType进行对象关系注册时如果没有指定生命周期管理器则默认使用这个生命周期管理器，这个生命周期管理器就如同其名字一样，当使用这种管理器的时候，每次通过Resolve或ResolveAll调用对象的时候都会重新创建一个新的对象。
             * 
             * ContainerControlledLifetimeManager，容器控制生命周期管理，这个生命周期管理器是RegisterInstance默认使用的生命周期管理器，也就是单件实例，UnityContainer会维护一个对象实例的强引用，每次调用的时候都会返回同一对象
             * 
            */

            _container = new UnityContainer();

            NameValueCollection nvc = ConfigurationManager.AppSettings;
            string serverList = nvc["ServerList"];
            string cachedArea = nvc["CachedArea"];
            string[] serverIp = serverList.Split(',');

            var ocs = nvc["OCS"];
            if (ocs == "true")
            {
                _container.RegisterType(typeof(ICache), typeof(OCSCache), new ContainerControlledLifetimeManager(), new InjectionConstructor(cachedArea));
            }
            else
            {
                _container.RegisterType(typeof(ICache), typeof(MemCache), new ContainerControlledLifetimeManager(), new InjectionConstructor(serverIp, cachedArea));
            }

            _container.RegisterType(typeof(IMySession), typeof(MySession), new ContainerControlledLifetimeManager());
            _container.RegisterType(typeof(ICache), typeof(AppCache), "AppCache", new ContainerControlledLifetimeManager());
            
            //注入当前登录用户
            _container.RegisterType<ICurrentUser, CurrentUser>(new ContainerControlledLifetimeManager());

            #region 新的数据访问方式
            string readConn = ConfigurationManager.ConnectionStrings["ReadDmsdev"].ConnectionString;
            string writeConn = ConfigurationManager.ConnectionStrings["WriteDmsdev"].ConnectionString;
            _container.RegisterType(typeof(IUnitOfWork), typeof(DefaultUnitOfWork), "ReadDmsdev", new InjectionConstructor(readConn));
            _container.RegisterType(typeof(IUnitOfWork), typeof(DefaultUnitOfWork), "WriteDmsdev", new InjectionConstructor(writeConn));
            #endregion

            #region 接口映射

            _container.RegisterType(typeof(LogService), typeof(LogService), new ContainerControlledLifetimeManager());
            
            _container.RegisterType(typeof(ConfigService), typeof(ConfigService), new ContainerControlledLifetimeManager());

            #endregion
        }
    }
}