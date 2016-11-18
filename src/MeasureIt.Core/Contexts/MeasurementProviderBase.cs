using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MeasurementProviderBase<TDiscoveryService> : IMeasurementProvider
        where TDiscoveryService : class, IRuntimeInstrumentationDiscoveryService
    {
        private readonly IInstrumentationDiscoveryOptions _options;

        private readonly Lazy<TDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the <see cref="TDiscoveryService"/> DiscoveryService.
        /// </summary>
        protected TDiscoveryService DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="discoveryService"></param>
        protected MeasurementProviderBase(IInstrumentationDiscoveryOptions options, TDiscoveryService discoveryService)
        {
            _options = options;

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<TDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                }, execAndPubThreadSafety);
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            var descriptors = DiscoveryService.Measurements
                .Where(
                    d => d.RootType.IsRelatedTo(targetType)
                         && d.Method.GetBaseDefinition() == method.GetBaseDefinition()).ToArray();

            var descriptor = descriptors.SingleOrDefault();

            var o = _options;

            return descriptor != null
                ? new MeasurementContext(o, descriptor, descriptor.CreateContext())
                : null;
        }
    }
}
