using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MeasureIt.AspNet.Mvc.Castle.Windsor.Controllers;

namespace MeasureIt.AspNet.Mvc.Castle.Windsor
{
    using Contexts;
    using Discovery;
    using Kingdom.Web.Mvc;
    using Web.Mvc.Castle.Windsor;
    using global::Castle.Windsor;

    public class MvcApplication : HttpApplication
    {
        private IWindsorContainer Container { get; }

        public MvcApplication()
        {
            Container = new WindsorContainer();
        }

        private static IEnumerable<Assembly> GetMeasurementAssemblies()
        {
            yield return typeof(MvcApplication).Assembly;
        }

        private static InstrumentationDiscoveryOptions CreateOptions()
        {
            return new InstrumentationDiscoveryOptions
            {
                ThrowOnInstallerFailure = false,
                ThrowOnUninstallerFailure = false,
                Assemblies = GetMeasurementAssemblies().ToArray()
            };
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Assumes filters, routes, bundles, etc, have all been configured.
            Container.InstallMvcServices<HomeController>(typeof(MeasurementContext))
                .UseDependencyResolver()
                .EnableMvcMeasurements<
                    IMvcActionInstrumentationDiscoveryService
                    , MvcActionInstrumentationDiscoveryService
                    , InstrumentationDiscoveryOptions>(CreateOptions)
                ;

#if DEBUG
            var kernel = Container.Kernel;

            Debug.Assert(kernel.HasComponent(typeof(IDependencyResolver)));
            Debug.Assert(kernel.HasComponent(typeof(IWindsorDependencyResolver)));
            Debug.Assert(kernel.HasComponent(typeof(IActionInvoker)));
#endif

            // TODO: TBD: need to connect with startup? warmup? Initialize the performance counters, install them... but don't forget to also uninstall them?

            using (var context = Container.Resolve<IInstallerInstrumentationDiscoveryService>()
                .GetInstallerContext())
            {
                context.Install();
            }
        }
    }
}
