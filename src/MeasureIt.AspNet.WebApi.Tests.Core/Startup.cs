using System.Web.Http;

namespace MeasureIt
{
    using Owin;
    using HttpStartup = Kingdom.Castle.Windsor.Web.Http.Startup;

    public partial class Startup : HttpStartup
    {
        protected virtual void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
        }

        public override void Configuration(IAppBuilder app)
        {
            base.Configuration(app);

            var config = Config;

            OnConfiguration(app, config);

            app.UseWebApi(config);
        }
    }
}
