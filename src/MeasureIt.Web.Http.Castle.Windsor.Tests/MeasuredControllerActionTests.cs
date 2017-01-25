using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Kingdom.Web.Http.Dependencies;
    using Xunit;
    using global::Castle.Windsor;

    public class MeasuredControllerActionTests
        : MeasuredControllerActionTestFixtureBase<
            IWindsorContainer, MeasuredStartupFixture>
    {
        protected override HttpConfiguration GetConfiguration()
        {
            var config = MeasuredStartupFixture.InternalConfig;

            // Verify a couple of handshakey things...
            Assert.NotNull(config);
            Assert.NotNull(config.DependencyResolver);
            Assert.IsType<WindsorDependencyResolver>(config.DependencyResolver);

            // TODO: TBD: ditto Autofac WebApi testing...
            return config;
        }
    }
}
