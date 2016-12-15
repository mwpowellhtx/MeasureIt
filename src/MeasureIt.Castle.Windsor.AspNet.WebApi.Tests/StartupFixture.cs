using System.Web.Http;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Kingdom.Castle.Windsor.Web.Http;
    using Owin;
    using global::Castle.Windsor;
    using HttpStartup = Startup;

    public class StartupFixture : HttpStartup
    {
        protected IWindsorContainer Container { get; private set; }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            config.ConfigureApi<StartupFixture>()
                .ConfigureDependencyResolver()
                .ContinueWith(container => Container = container);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});
        }
    }
}
