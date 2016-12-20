using System.Web.Http;

namespace MeasureIt.Autofac.AspNet.WebApi
{
    using Xunit;
    using global::Autofac.Integration.WebApi;

    public class MeasuredControllerActionTests : MeasuredControllerActionTestFixtureBase<MeasuredStartupFixture>
    {
        protected override HttpConfiguration GetConfiguration()
        {
            var config = MeasuredStartupFixture.InternalConfig;

            // Verify a couple of handshakey things...
            Assert.NotNull(config);
            Assert.NotNull(config.DependencyResolver);
            Assert.IsType<AutofacWebApiDependencyResolver>(config.DependencyResolver);

            Assert.NotNull(config);

            // TODO: TBD: is there a better generally acceptable way to get at this config? or otherwise build up a server such that we have access to the startup config?
            return config;
        }
    }
}
