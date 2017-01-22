using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Discovery;
    using Kingdom.Web.Http.Dependencies;
    using Xunit;

    public class MeasuredControllerActionTests : MeasuredControllerActionTestFixtureBase<BasicMeasuredStartupFixture>
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

    /// <summary>
    /// Apparently self-hosted OWIN servers must provide a concrete class, no generics involved.
    /// Although it may derive from a generic base class as we do here.
    /// </summary>
    public class BasicMeasuredStartupFixture : MeasuredStartupFixture<InstrumentationDiscoveryOptions>
    {
    }
}
