using System.Web.Http;

namespace MeasureIt
{
    using Owin;

    //// TODO: TBD: actually, for what this is, I'm not sure it justifies a dependency on a DI specific package, after all...
    //using HttpStartup = Kingdom.Castle.Windsor.Web.Http.Startup;

    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        // TODO: TBD: may need/want to replace the Config downstream from here...
        protected HttpConfiguration Config { get; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
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
            OnConfiguration(app, Config);

            app.UseWebApi(Config);
        }
    }
}
