using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Castle.Interception.Measurement
{
    using Contexts;
    using Discovery;

    public class InterceptionMeasurementProvider
        : MeasurementProviderBase<IInterceptionMeasurementContext>
            , IInterceptionMeasurementProvider
    {
        private readonly Lazy<IRuntimeInstrumentationDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the DiscoveryService.
        /// </summary>
        protected IRuntimeInstrumentationDiscoveryService DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

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
