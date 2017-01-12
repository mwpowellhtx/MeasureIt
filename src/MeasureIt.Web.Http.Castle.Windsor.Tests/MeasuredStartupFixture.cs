using System.Web.Http;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Controllers;
    using Discovery;
    using Interception;
    using Owin;

    public class MeasuredStartupFixture<TOptions> : StartupFixture
        where TOptions : class, IInstrumentationDiscoveryOptions, new()
    {
        protected virtual TOptions GetDiscoveryOptions()
        {
            return new TOptions
            {
                ThrowOnInstallerFailure = false,
                ThrowOnUninstallerFailure = false,
                Assemblies = new[]
                {
                    typeof(MeasuredController).Assembly
                    , typeof(AverageTimePerformanceCounterAdapter).Assembly
                }

            };
        }

        // ReSharper disable once StaticMemberInGenericType
        internal static HttpConfiguration InternalConfig { get; private set; }

        private void Install<TDiscoveryService>()
            where TDiscoveryService : class, IInstallerInstrumentationDiscoveryService
        {
            var discoveryService = Container.Resolve<TDiscoveryService>();

            // TODO: TBD: in a production environment, this should probably go in a true installer of some sort...
            using (var context = discoveryService.GetInstallerContext())
            {
                context.Install();
            }
        }

        protected override void OnConfiguration(IAppBuilder app, HttpConfiguration config)
        {
            /* TODO: TBD: got this pretty far today: now we have a situation where the counter naming convention reveals a conflict over method names/resolutions...
             * Should be easy to resolve with a better strategy, or one that is sensitive to the full signature... */

            // We need to configure basic startup features as well.
            base.OnConfiguration(app, config);

            InternalConfig = config;

            // Followed by enabling Api measurements via Container.
            Container.EnableApiMeasurements<
                IHttpActionInstrumentationDiscoveryService
                , HttpActionInstrumentationDiscoveryService
                , TOptions
                , HttpActionMeasurementProvider>(GetDiscoveryOptions);

            Install<IInstallerInstrumentationDiscoveryService>();
            Install<IHttpActionInstrumentationDiscoveryService>();
        }

        // TODO: TBD: OnDispose?
    }
}
