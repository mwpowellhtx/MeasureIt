using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MeasureIt.Discovery
{
    using Agents;
    using DataAttribute = CounterCreationDataAttribute;

    // TODO: TBD: copy (or inherit) from this one extending into WebApi (and later, Mvc)...
    /// <summary>
    /// 
    /// </summary>
    public class HttpActionInstrumentationDiscoveryService : InstrumentationDiscoveryServiceBase
        , IHttpActionInstrumentationDiscoveryService
    {
        private readonly Lazy<IMeasurementFilterDiscoveryAgent> _lazyMeasurementFilterDiscoveryAgent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public HttpActionInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
            PrivateMeasurements = null;

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyMeasurementFilterDiscoveryAgent = new Lazy<IMeasurementFilterDiscoveryAgent>(
                () => new MeasurementFilterDiscoveryAgent(options, GetExportedTypes), execAndPubThreadSafety);
        }

        private IEnumerable<IPerformanceMeasurementDescriptor> _measurements;

        public override IEnumerable<IPerformanceMeasurementDescriptor> Measurements
        {
            get { return _measurements; }
        }

        private IEnumerable<IPerformanceMeasurementDescriptor> PrivateMeasurements
        {
            set { _measurements = (value ?? new List<IPerformanceMeasurementDescriptor>()).ToArray(); }
        }

        private void OnDiscoverMeasurementFilterPerformanceDescriptors()
        {
            PrivateMeasurements = _lazyMeasurementFilterDiscoveryAgent.Value.ToArray();
        }

        /// <summary>
        /// Discover handler.
        /// </summary>
        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverMeasurementFilterPerformanceDescriptors();
        }
    }
}
