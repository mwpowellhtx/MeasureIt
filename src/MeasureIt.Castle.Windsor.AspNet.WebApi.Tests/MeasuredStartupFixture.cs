using System.Web.Http;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Discovery;
    using Owin;
    using Web.Http.Interception;

    public class MeasuredStartupFixture : StartupFixture
    {
        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            /* TODO: TBD: got this pretty far today: now we have a situation where the counter naming convention reveals a conflict over method names/resolutions...
             * Should be easy to resolve with a better strategy, or one that is sensitive to the full signature... */

            // We need to configure basic startup features as well.
            base.OnConfiguration(app, config);

            // Followed by enabling Api measurements via Container.
            Container.EnableApiMeasurements<
                IHttpActionInstrumentationDiscoveryService
                , HttpActionInstrumentationDiscoveryService
                , HttpActionMeasurementProvider>(o =>
                {
                    // TODO: TBD: not expecting installer to fail, per se
                    // TODO: TBD: there are doubts whether we need to flag this after all...
                    o.ThrowOnInstallerFailure = false;

                    o.Assemblies = new[]
                    {
                        typeof(MeasuredStartupFixture).Assembly
                        , typeof(AverageTimePerformanceCounterAdapter).Assembly
                    };
                });

            /* TODO: TBD: this is as good a time as any to install things...
             * TODO: TBD: not sure this would be right in a production setting, however, apart from a true installer... */
            var discoveryService = Container.Resolve<IInstallerInstrumentationDiscoveryService>();

            using (var context = discoveryService.GetInstallerContext())
            {
                context.Install();
            }
        }
    }
}
