using System.Web.Http;

namespace MeasureIt.Web.Http.Autofac
{
    using Controllers;
    using Discovery;
    using Interception;
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


        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            InternalConfig = config;

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(MeasuredController).Assembly);

            builder.EnableApiMeasurements<
                IHttpActionInstrumentationDiscoveryService
                , HttpActionInstrumentationDiscoveryService
                , HttpActionMeasurementProvider>(o =>
                {
                    //// TODO: TBD: not expecting installer to fail, per se
                    //// TODO: TBD: there are doubts whether we need to flag this after all...
                    o.ThrowOnInstallerFailure = false;

                    o.Assemblies = new[]
                    {
                        typeof(MeasuredController).Assembly
                        , typeof(AverageTimePerformanceCounterAdapter).Assembly
                    };
                });

            var container = Container = builder.Build();

            Install<IInstallerInstrumentationDiscoveryService>();
            Install<IHttpActionInstrumentationDiscoveryService>();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});
        }
    }
}
