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
        private readonly Lazy<IMeasurePerformanceDescriptorDiscoveryAgent> _performanceCounterDescriptorDiscoveryAgent;

        private IEnumerable<IMeasurePerformanceDescriptor> _counterDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IMeasurePerformanceDescriptor> CounterDescriptors
        {
            get { return _counterDescriptors; }
            private set { _counterDescriptors = (value ?? new List<IMeasurePerformanceDescriptor>()).ToArray(); }
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
                = new Lazy<IMeasurePerformanceDescriptorDiscoveryAgent>(
                    () => new MeasurePerformanceDescriptorDiscoveryAgent(options, GetExportedTypes));
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
