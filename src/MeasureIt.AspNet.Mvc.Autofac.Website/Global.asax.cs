﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MeasureIt.AspNet.Mvc.Autofac
{
    using Controllers;
    using Discovery;
    using Web.Mvc.Autofac;
    using global::Autofac;
    using global::Autofac.Integration.Mvc;

    public class MvcApplication : HttpApplication
    {
        private ContainerBuilder Builder { get; }

        public MvcApplication()
        {
            Builder = new ContainerBuilder();
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

        private static IEnumerable<Assembly> GetControllerAssemblies()
        {
            yield return typeof(HomeController).Assembly;
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = Builder;

            // Assumes filters, routes, bundles, etc, have all been configured.

            // TODO: TBD: could/should potentially put these in a useful extension method...
            builder.RegisterRequiredServices<AutofacDependencyResolver>();

            builder.RegisterControllers(GetControllerAssemblies().ToArray()).InjectActionInvoker();

            builder.EnableMvcMeasurements<
                    IMvcActionInstrumentationDiscoveryService
                    , MvcActionInstrumentationDiscoveryService
                    , InstrumentationDiscoveryOptions>(CreateOptions)
                ;

            // Build the Container and Resolve the Resolver.
            var container = builder.Build();

            DependencyResolver.SetResolver(container.Resolve<IDependencyResolver>());

            /* Obviously, in a more production oriented environment, we would not necessarily want
             * to install the Measurement Context like this. */

            using (var context = container.Resolve<IInstallerInstrumentationDiscoveryService>()
                .GetInstallerContext())
            {
                context.Install();
            }
        }
    }
}
