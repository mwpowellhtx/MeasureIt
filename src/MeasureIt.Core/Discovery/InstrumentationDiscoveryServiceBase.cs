﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

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

        private readonly Lazy<IPerformanceCounterAdapterDiscoveryAgent> _lazyCounterAdapterDiscoveryAgent;

        protected readonly DiscoveryServiceExportedTypesGetterDelegate GetExportedTypes;

        private IEnumerable<IPerformanceCounterAdapter> _counterAdapters;

        public abstract IEnumerable<IPerformanceMeasurementDescriptor> Measurements { get; }

        /// <summary>
        /// Gets the CounterAdapterDescriptors.
        /// These will be discovered in order to substantiate any Measurement claims.
        /// </summary>
        protected IEnumerable<IPerformanceCounterAdapter> CounterAdapters
        {
            get { return _counterAdapters; }
            private set { _counterAdapters = (value ?? new List<IPerformanceCounterAdapter>()).ToArray(); }
        }

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

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyCounterAdapterDiscoveryAgent = new Lazy<IPerformanceCounterAdapterDiscoveryAgent>(
                () => new PerformanceCounterAdapterDiscoveryAgent(options, GetExportedTypes), execAndPubThreadSafety);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPending { get; private set; }

        private void OnDiscoverPerformanceCounterAdapters()
        {
            CounterAdapters = _lazyCounterAdapterDiscoveryAgent.Value.ToArray();
        }

        /// <summary>
        /// <see cref="Discover"/> handler.
        /// </summary>
        protected virtual void OnDiscover()
        {
            OnDiscoverPerformanceCounterAdapters();
        }

        /// <summary>
        /// Discovered event.
        /// </summary>
        public event EventHandler<EventArgs> Discovered;

        /// <summary>
        /// Signals when <see cref="Discovered"/>.
        /// </summary>
        protected virtual void OnDiscovered()
        {
            if (Discovered == null) return;
            Discovered(this, EventArgs.Empty);
        }

        /// <summary>
        /// Discovers the Performance Monitoring Instrumentation from the <see cref="Assemblies"/>.
        /// </summary>
        public void Discover()
        {
            if (!IsPending) return;

            OnDiscover();

            IsPending = !IsPending;

            OnDiscovered();
        }
    }
}
