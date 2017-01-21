using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Web.Mvc.Interception
{
    using Contexts;
    using Discovery;
    using static LazyThreadSafetyMode;

    /// <summary>
    /// Measurement Provider for web purposes.
    /// </summary>
    public class MvcActionMeasurementProvider : MeasurementProviderBase<
        ITwoStageMeasurementContext>, ITwoStageMeasurementProvider
    {
        private readonly Lazy<IMvcActionInstrumentationDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the DiscoveryService.
        /// </summary>
        protected IMvcActionInstrumentationDiscoveryService DiscoveryService => _lazyDiscoveryService.Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="discoveryService"></param>
        public MvcActionMeasurementProvider(IMvcInstrumentationDiscoveryOptions options
            , IMvcActionInstrumentationDiscoveryService discoveryService)
            : base(options)
        {
            _lazyDiscoveryService = new Lazy<IMvcActionInstrumentationDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                }, ExecutionAndPublication);
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
