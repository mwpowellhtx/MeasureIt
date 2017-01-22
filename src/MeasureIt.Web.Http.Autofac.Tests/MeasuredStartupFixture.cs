using System.Web.Http;
using System.Web.Http.Dependencies;

namespace MeasureIt.Web.Http.Autofac
{
    using Controllers;
    using Discovery;
    using Owin;
    using global::Autofac;
    using global::Autofac.Integration.WebApi;
    using MeasureItStartup = Startup;

    public class MeasuredStartupFixture : MeasureItStartup
    {
        internal static HttpConfiguration InternalConfig { get; private set; }

        protected IContainer Container { get; private set; }

        private void Install<TDiscoveryService>()
            where TDiscoveryService : class, IInstallerInstrumentationDiscoveryService
        {
            var discoveryService = Container.Resolve<TDiscoveryService>();

            // TODO: TBD: in a production environment, this should probably go in a true installer of some sort...
            using (var context = discoveryService.GetInstallerContext())
            {
                context.Install();
            }
        }

        private static InstrumentationDiscoveryOptions CreateDiscoveryOptions()
        {
            return new InstrumentationDiscoveryOptions
            {
                ThrowOnInstallerFailure = false,
                ThrowOnUninstallerFailure = false,
                Assemblies = new[]
                {
                    typeof(MeasuredController).Assembly
                    , typeof(AverageTimePerformanceCounterAdapter).Assembly
                }
            };
        }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            InternalConfig = config;

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(MeasuredController).Assembly);

            builder.EnableApiMeasurements<
                    IHttpActionInstrumentationDiscoveryService
                    , HttpActionInstrumentationDiscoveryService
                    , InstrumentationDiscoveryOptions>(CreateDiscoveryOptions)
                .UseAutofacApiDependencyResolver<AutofacWebApiDependencyResolver>()
                ;

            var container = Container = builder.Build();

            /* TODO: TBD: code such as this, Install<?>() perhaps goes better in a dedicated installer,
             * or at a bare minimum quarantined behind a decidated server side controller. */

            Install<IInstallerInstrumentationDiscoveryService>();
            Install<IHttpActionInstrumentationDiscoveryService>();

            // Very nearly last but not least inform the configuration of our Dependency Resolver.
            config.DependencyResolver = container.Resolve<IDependencyResolver>();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});
        }
    }
}
