using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Discovery
{
    using Agents;
    using Contexts;

    // TODO: TBD: copy (or inherit) from this one extending into WebApi (and later, Mvc)...
    /// <summary>
    /// Discovery service for purposes of supporting Runtime Instrumentation.
    /// </summary>
    public class RuntimeInstrumentationDiscoveryService : InstrumentationDiscoveryServiceBase
        , IRuntimeInstrumentationDiscoveryService
    {
        private readonly Lazy<IPerformanceMeasurementDescriptorDiscoveryAgent> _lazyMeasurementDiscoveryAgent;

        private IEnumerable<IPerformanceMeasurementDescriptor> _measurements;

        /// <summary>
        /// Gets the Measurements corresponding with the Discovery service.
        /// </summary>
        public override IEnumerable<IPerformanceMeasurementDescriptor> Measurements => _measurements;

        private IEnumerable<IPerformanceMeasurementDescriptor> PrivateMeasurements
        {
            set { _measurements = (value ?? new List<IPerformanceMeasurementDescriptor>()).ToArray(); }
        }

        /// <summary>
        /// Gets the CategoryAdapters corresponding with the DiscoveryService.
        /// </summary>
        public IDictionary<Type, IPerformanceCounterCategoryAdapter> CategoryAdapters { get; }
            = new ConcurrentDictionary<Type, IPerformanceCounterCategoryAdapter>();

        private void RegisterCategoryAdapters(
            IDictionary<Type, IPerformanceCounterCategoryAdapter> adapters
            , IEnumerable<IPerformanceMeasurementDescriptor> measurements)
        {
            adapters.Clear();

            var bindingAttr = Options.ConstructorBindingAttr;

            // We want to consolidate the Counter Descriptors under single instances of each Category Adapter Type.
            foreach (var m in measurements)
            {
                var t = m.CategoryType;

                var adapter = adapters.ContainsKey(t)
                    ? adapters[t]
                    : (adapters[t] = t.CreateInstance<IPerformanceCounterCategoryAdapter>(bindingAttr));

                adapter.Register(m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RuntimeInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
            PrivateMeasurements = null;

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyMeasurementDiscoveryAgent = new Lazy<IPerformanceMeasurementDescriptorDiscoveryAgent>(
                () => new PerformanceMeasurementDescriptorDiscoveryAgent(options, GetExportedTypes), execAndPubThreadSafety);
        }

        private void OnDiscoverMeasurePerformanceDescriptors()
        {
            PrivateMeasurements = _lazyMeasurementDiscoveryAgent.Value.ToArray();
        }

        /// <summary>
        /// Discoverey handler.
        /// </summary>
        protected override void OnDiscovered()
        {
            base.OnDiscovered();

            OnDiscoverMeasurePerformanceDescriptors();

            RegisterCategoryAdapters(CategoryAdapters, Measurements);
        }
    }
}
