using System.Web.Http;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Kingdom.Castle.Windsor.Web.Http;
    using Owin;
    using HttpStartup = Startup;

    public class StartupFixture : HttpStartup
    {
        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            base.OnConfiguration(app, config);

            config.ConfigureApi<StartupFixture>()
                .ConfigureDependencyResolver();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});
        }
    }
}
