using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Kingdom.Web.Http;
    using Owin;
    using global::Castle.Windsor;

    public class StartupFixture : Startup<IWindsorContainer>
    {
        public StartupFixture()
        {
            // Make sure we have a Container waiting for us OnConfiguration.
            Container = new WindsorContainer();
        }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            Container.InstallApiServices<StartupFixture>(config);

            config.UseWindsorDependencyResolver(Container);

            base.OnConfiguration(app, config);
        }
    }
}
