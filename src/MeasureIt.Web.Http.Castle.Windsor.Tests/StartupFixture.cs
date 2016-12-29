using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Kingdom.Web.Http;
    using Owin;
    using global::Castle.Windsor;
    using StartupBase = Startup;

    public class StartupFixture : StartupBase
    {
        /// <summary>
        /// Gets the Container associated with the StartupFixture.
        /// </summary>
        protected IWindsorContainer Container { get; }

        public StartupFixture()
        {
            // Make sure we have a Container waiting for us OnConfiguration.
            Container = new WindsorContainer();
        }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            Container.ConfigureApi<StartupFixture>(config);

            config.UseWindsorDependencyResolver(Container)
                .MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});
        }
    }
}
