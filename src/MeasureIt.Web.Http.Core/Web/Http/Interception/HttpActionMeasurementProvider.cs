using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Web.Http.Interception
{
    using Contexts;
    using Discovery;

    /// <summary>
    /// Measurement Provider for web purposes.
    /// </summary>
    public class HttpActionMeasurementProvider : MeasurementProviderBase<
        ITwoStageMeasurementContext>, ITwoStageMeasurementProvider
    {
        private readonly Lazy<IHttpActionInstrumentationDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the DiscoveryService.
        /// </summary>
        protected IHttpActionInstrumentationDiscoveryService DiscoveryService => _lazyDiscoveryService.Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="discoveryService"></param>
        public HttpActionMeasurementProvider(IInstrumentationDiscoveryOptions options
            , IHttpActionInstrumentationDiscoveryService discoveryService)
            : base(options)
        {
            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<IHttpActionInstrumentationDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                }, execAndPubThreadSafety);
        }

        /// <summary>
        /// Returns the Measurement Context given <paramref name="targetType"/> and
        /// <paramref name="method"/>.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override ITwoStageMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            var descriptors = DiscoveryService.Measurements
                .Where(
                    d => d.RootType.IsRelatedTo(targetType)
                         && d.Method.GetBaseDefinition() == method.GetBaseDefinition()).ToArray();

            var descriptor = descriptors.SingleOrDefault();

            var o = Options;

            return descriptor != null
                ? new TwoStageMeasurementContext(o, descriptor, descriptor.CreateContext())
                : null;
        }
    }
}
