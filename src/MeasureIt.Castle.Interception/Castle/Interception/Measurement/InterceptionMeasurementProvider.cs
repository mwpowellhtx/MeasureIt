using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Castle.Interception.Measurement
{
    using Contexts;
    using Discovery;

    /// <summary>
    /// Measurement provider for Interception purposes.
    /// </summary>
    public class InterceptionMeasurementProvider
        : MeasurementProviderBase<IInterceptionMeasurementContext>
            , IInterceptionMeasurementProvider
    {
        private readonly Lazy<IRuntimeInstrumentationDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the DiscoveryService.
        /// </summary>
        protected IRuntimeInstrumentationDiscoveryService DiscoveryService => _lazyDiscoveryService.Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="discoveryService"></param>
        public InterceptionMeasurementProvider(IInstrumentationDiscoveryOptions options
            , IRuntimeInstrumentationDiscoveryService discoveryService)
            : base(options)
        {
            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<IRuntimeInstrumentationDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                }, execAndPubThreadSafety);
        }

        /// <summary>
        /// Returns a Context corresponding with the Provider.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public override IInterceptionMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            var descriptors = DiscoveryService.Measurements
                .Where(
                    d => d.RootType.IsRelatedTo(targetType)
                         && d.Method.GetBaseDefinition() == method.GetBaseDefinition()).ToArray();

            var descriptor = descriptors.SingleOrDefault();

            var o = Options;

            return descriptor != null
                ? new MeasurementContext(o, descriptor, descriptor.CreateContext())
                : null;
        }
    }
}
