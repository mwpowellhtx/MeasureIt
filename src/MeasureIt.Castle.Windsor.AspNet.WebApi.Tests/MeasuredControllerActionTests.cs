using System.Web.Http;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Kingdom.Castle.Windsor.Web.Http.Dependencies;
    using Xunit;

    public class MeasuredControllerActionTests : MeasuredControllerActionTestFixtureBase<MeasuredStartupFixture>
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
