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
        private readonly HttpConfiguration _config;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Startup()
        {
            _config = new HttpConfiguration();
        }

        /// <summary>
        /// Gets the <see cref="HttpConfiguration"/> associated with this Startup.
        /// </summary>
        protected HttpConfiguration Config
        {
            get { return _config; }
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

            app.UseWebApi(config);
        }
    }
}
