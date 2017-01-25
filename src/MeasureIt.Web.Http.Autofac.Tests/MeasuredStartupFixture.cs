using System.Web.Http;
using System.Web.Http.Dependencies;

namespace MeasureIt.Web.Http.Autofac
{
    using Controllers;
    using Discovery;
    using Owin;
    using global::Autofac;
    using global::Autofac.Integration.WebApi;

    public class MeasuredStartupFixture : Startup<IContainer>
    {
        internal static HttpConfiguration InternalConfig { get; private set; }

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
            InternalConfig = config;

            var builder = new ContainerBuilder();

#pragma warning disable 612

            builder.RegisterApiServices<
                    AutofacWebApiDependencyResolver
                    , AutofacHttpControllerActivator
                    , TraceExceptionLogger>()
                .EnableApiMeasurements<
                    IHttpActionInstrumentationDiscoveryService
                    , HttpActionInstrumentationDiscoveryService>(CreateDiscoveryOptions)
                .RegisterApiControllers(typeof(MeasuredController).Assembly)
                ;

#pragma warning restore 612

            var container = Container = builder.Build();

            // Very nearly last but not least inform the configuration of our Dependency Resolver.
            config.DependencyResolver = container.Resolve<IDependencyResolver>();

            /* TODO: TBD: code such as this, Install<?>() perhaps goes better in a dedicated installer,
             * or at a bare minimum quarantined behind a decidated server side controller. */

            Install<IInstallerInstrumentationDiscoveryService>();
            Install<IHttpActionInstrumentationDiscoveryService>();

            base.OnConfiguration(app, config);
        }
    }
}
