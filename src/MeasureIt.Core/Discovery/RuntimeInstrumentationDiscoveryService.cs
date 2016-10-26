using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Agents;
    using Measurement;
    using DataAttribute = CounterCreationDataAttribute;

    /// <summary>
    /// 
    /// </summary>
    public class RuntimeInstrumentationDiscoveryService : InstrumentationDiscoveryServiceBase
        , IRuntimeInstrumentationDiscoveryService
    {
        private readonly Lazy<IPerformanceCounterDescriptorDiscoveryAgent> _performanceCounterDescriptorDiscoveryAgent;

        private IEnumerable<IPerformanceCounterDescriptor> _counterDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPerformanceCounterDescriptor> CounterDescriptors
        {
            get { return _counterDescriptors; }
            private set { _counterDescriptors = (value ?? new List<IPerformanceCounterDescriptor>()).ToArray(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RuntimeInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : this(options, new List<Assembly>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblies"></param>
        public RuntimeInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options,
            IEnumerable<Assembly> assemblies)
            : base(options, assemblies)
        {
            CounterDescriptors = null;

            _performanceCounterDescriptorDiscoveryAgent
                = new Lazy<IPerformanceCounterDescriptorDiscoveryAgent>(
                    () => new PerformanceCounterDescriptorDiscoveryAgent(options, GetExportedTypes));
        }

        /* TODO: TBD: we may need/want different discovery services for different purposes:
         * i.e. does a runtime discovery service really require the category descriptors?
         * that's just for install/uninstall purposes methinks... */

        private void OnDiscoverCounterDescriptors()
        {
            CounterDescriptors = _performanceCounterDescriptorDiscoveryAgent.Value.ToArray();
        }

        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverCounterDescriptors();
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo methodInfo)
        {
            return new MeasurementContext(null, null);
        }
    }
}
