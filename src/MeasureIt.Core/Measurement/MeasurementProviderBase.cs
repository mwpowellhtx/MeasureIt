using System;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Measurement
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

            _lazyDiscoveryService = new Lazy<TDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                });
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            // TODO: TBD: everything else seems pretty much boilerplate; I believe aligning descriptors with contexts hinges on this.

            var descriptors = DiscoveryService.CounterDescriptors.Where(
                d => d.RootType.IsRelatedTo(targetType)
                     && d.Method.GetBaseDefinition() == method.GetBaseDefinition()).ToArray();

            var o = _options;

            // TODO: TBD: this one has me wondering, there should be some sort of factory on the descriptor(s) around this

            return descriptors.Any()
                ? new MeasurementContext(o, descriptors.Select(d => d.CreateContext()))
                : null;
        }
    }
}
