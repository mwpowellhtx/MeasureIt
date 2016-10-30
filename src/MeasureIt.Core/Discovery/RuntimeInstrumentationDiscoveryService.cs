using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Agents;
    using Contexts;
    using DataAttribute = CounterCreationDataAttribute;

    /// <summary>
    /// 
    /// </summary>
    public class RuntimeInstrumentationDiscoveryService : InstrumentationDiscoveryServiceBase
        , IRuntimeInstrumentationDiscoveryService
    {
        private readonly Lazy<IPerformanceMeasurementDescriptorDiscoveryAgent> _performanceCounterDescriptorDiscoveryAgent;

        private IEnumerable<IPerformanceMeasurementDescriptor> _measurePerformanceDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPerformanceMeasurementDescriptor> MeasurementDescriptors
        {
            get { return _measurePerformanceDescriptors; }
            private set { _measurePerformanceDescriptors = (value ?? new List<IPerformanceMeasurementDescriptor>()).ToArray(); }
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
            MeasurementDescriptors = null;

            _performanceCounterDescriptorDiscoveryAgent
                = new Lazy<IPerformanceMeasurementDescriptorDiscoveryAgent>(
                    () => new PerformanceMeasurementDescriptorDiscoveryAgent(options, GetExportedTypes));
        }

        /* TODO: TBD: we may need/want different discovery services for different purposes:
         * i.e. does a runtime discovery service really require the category descriptors?
         * that's just for install/uninstall purposes methinks... */

        private void OnDiscoverCounterDescriptors()
        {
            MeasurementDescriptors = _performanceCounterDescriptorDiscoveryAgent.Value.ToArray();
        }

        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverCounterDescriptors();
        }

        public IMeasurementContext GetMeasurementContext()
        {
            return null;
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo methodInfo)
        {
            return new MeasurementContext(null, null);
        }
    }
}
