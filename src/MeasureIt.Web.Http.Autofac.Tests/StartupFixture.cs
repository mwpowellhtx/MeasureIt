using System.Web.Http;
using System.Web.Http.Dependencies;

namespace MeasureIt.Web.Http.Autofac
{
    using Controllers;
    using Owin;
    using global::Autofac;
    using global::Autofac.Integration.WebApi;

    public class StartupFixture : Startup<IContainer>
    {
        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // TODO: TBD: consider whether this belongs as an extension method?

#pragma warning disable 612

            builder.RegisterApiServices<
                    AutofacWebApiDependencyResolver
                    , AutofacHttpControllerActivator
                    , TraceExceptionLogger>()
                .RegisterApiControllers(typeof(MeasuredController).Assembly);

#pragma warning restore 612

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

            base.OnConfiguration(app, config);

            var container = Container = builder.Build();

            app.UseAutofacWebApi(config)
                .UseAutofacMiddleware(container);

            config.DependencyResolver = container.Resolve<IDependencyResolver>();
       }
    }
}
