using System.Web.Http;

namespace MeasureIt.Web.Http
{
    using Owin;

    //// TODO: TBD: actually, for what this is, I'm not sure it justifies a dependency on a DI specific package, after all...
    //using HttpStartup = Kingdom.Castle.Windsor.Web.Http.Startup;

    /// <summary>
    /// 
    /// </summary>
    public abstract partial class Startup<TContainer>
    {
        // TODO: TBD: may need/want to replace the Config downstream from here...
        protected HttpConfiguration Config { get; }

        protected TContainer Container { get; set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        protected Startup()
        {
            // Make sure that we have an HttpConfiguration ready and waiting for us.
            Config = new HttpConfiguration();
        }

        protected virtual void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
        }

        /// <summary>
        /// Configures Startup based on the <paramref name="app"/>.
        /// </summary>
        /// <param name="app"></param>
        public virtual void Configuration(IAppBuilder app)
        {
            var config = Config;

            OnConfiguration(app, config);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi", "api/{controller}/{action}/{value}",
                new {action = "get", value = RouteParameter.Optional});

            app.UseWebApi(Config);
        }
    }
}
