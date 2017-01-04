using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MeasureIt.AspNet.Mvc.Castle.Windsor
{
    using Controllers;
    using Kingdom.Web.Mvc;
    using global::Castle.Windsor;

    public class MvcApplication : HttpApplication
    {
        private IWindsorContainer Container { get; }

        public MvcApplication()
        {
            Container = new WindsorContainer();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Assumes filters, routes, bundles, etc, have all been configured.
            Container.InstallMvcServices<HomeController>()
                .UseDependencyResolver()
                ;
        }
    }
}
