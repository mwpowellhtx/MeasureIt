using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Agents;

    /// <summary>
    /// InstrumentationDiscoveryServiceBase base class.
    /// </summary>
    public abstract class InstrumentationDiscoveryServiceBase : IInstrumentationDiscoveryService
    {
        /// <summary>
        /// Gets the Assemblies from which to Discover any Instrumentation.
        /// </summary>
        protected IEnumerable<Assembly> Assemblies
        {
            get { return Options.Assemblies; }
        }

        private readonly Lazy<IPerformanceCounterAdapterDescriptorDiscoveryAgent>
            _performanceCounterAdapterDescriptorDiscoveryAgent;

        private IEnumerable<IPerformanceCounterAdapterDescriptor> _counterAdapterDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPerformanceCounterAdapterDescriptor> CounterAdapterDescriptors
        {
            get { return _counterAdapterDescriptors; }
            private set
            {
                _counterAdapterDescriptors = (value ?? new List<IPerformanceCounterAdapterDescriptor>()).ToArray();
            }
        }

        protected readonly DiscoveryServiceExportedTypesGetterDelegate GetExportedTypes;

        /// <summary>
        /// Gets the Options.
        /// </summary>
        protected IInstrumentationDiscoveryOptions Options { get; private set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="options"></param>
        protected InstrumentationDiscoveryServiceBase(IInstrumentationDiscoveryOptions options)
        {
            IsPending = true;
            Options = options;

            GetExportedTypes = () => Assemblies.SelectMany(a => a.GetExportedTypes());

            CounterAdapterDescriptors = null;

            _performanceCounterAdapterDescriptorDiscoveryAgent
                = new Lazy<IPerformanceCounterAdapterDescriptorDiscoveryAgent>(
                    () => new PerformanceCounterAdapterDescriptorDiscoveryAgent(options, GetExportedTypes));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPending { get; private set; }

        private void OnDiscoverCounterCategoryDescriptors()
        {
            CounterAdapterDescriptors = _performanceCounterAdapterDescriptorDiscoveryAgent.Value.ToArray();
        }

        /// <summary>
        /// <see cref="Discover"/> handler.
        /// </summary>
        protected virtual void OnDiscover()
        {
            OnDiscoverCounterCategoryDescriptors();
        }

        /// <summary>
        /// Discovers the Performance Monitoring Instrumentation from the <see cref="Assemblies"/>.
        /// </summary>
        public void Discover()
        {
            if (!IsPending) return;

            OnDiscover();

            IsPending = !IsPending;
        }
    }
}
