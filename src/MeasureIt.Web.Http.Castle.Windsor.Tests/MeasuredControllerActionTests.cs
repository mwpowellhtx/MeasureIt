using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Discovery;
    using Kingdom.Web.Http.Dependencies;
    using Xunit;

    public class MeasuredControllerActionTests
        : MeasuredControllerActionTestFixtureBase<
            MeasuredStartupFixture<InstrumentationDiscoveryOptions>
        >
    {
        protected override HttpConfiguration GetConfiguration()
        {
            var config = MeasuredStartupFixture<InstrumentationDiscoveryOptions>.InternalConfig;

            // Verify a couple of handshakey things...
            Assert.NotNull(config);
            Assert.NotNull(config.DependencyResolver);
            Assert.IsType<WindsorDependencyResolver>(config.DependencyResolver);

            // TODO: TBD: ditto Autofac WebApi testing...
            return config;
        }
    }
}
