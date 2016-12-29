using System.Web.Http;

namespace MeasureIt.Web.Http.Autofac
{
    using Controllers;
    using Owin;
    using global::Autofac;
    using global::Autofac.Integration.WebApi;
    using MeasureItStartup = Startup;

    public class StartupFixture : MeasureItStartup
    {
        protected IContainer Container { get; private set; }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(typeof(MeasuredController).Assembly);

            //builder.EnableApiMeasurements<
            //    IHttpActionInstrumentationDiscoveryService
            //    , HttpActionInstrumentationDiscoveryService
            //    , HttpActionMeasurementProvider>(o =>
            //    {
            //        //// TODO: TBD: not expecting installer to fail, per se
            //        //// TODO: TBD: there are doubts whether we need to flag this after all...
            //        //o.ThrowOnInstallerFailure = false;

            //        //o.Assemblies = new[]
            //        //{
            //        //    typeof(MeasuredStartupFixture).Assembly
            //        //    , typeof(AverageTimePerformanceCounterAdapter).Assembly
            //        //};
            //    });

            var container = Container = builder.Build();

            app.UseAutofacLifetimeScopeInjector(container);

            //app.UseAutofacMiddleware(container);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});

            app.UseWebApi(config);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
