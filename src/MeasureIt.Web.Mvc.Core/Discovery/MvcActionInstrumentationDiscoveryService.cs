using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MeasureIt.Web.Mvc.Discovery
{
    using Agents;
    using Contexts;
    using MeasureIt.Discovery;
    using static LazyThreadSafetyMode;

    /// <summary>
    /// Instrumentation discovery service for web purposes.
    /// </summary>
    public class MvcActionInstrumentationDiscoveryService : RuntimeInstrumentationDiscoveryService
        , IMvcActionInstrumentationDiscoveryService
    {
        private readonly Lazy<IMeasurementFilterDiscoveryAgent> _lazyMeasurementFilterDiscoveryAgent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discoveryOptions"></param>
        public MvcActionInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions discoveryOptions)
            : base(discoveryOptions)
        {
            PrivateMeasurements = null;

            _lazyMeasurementFilterDiscoveryAgent = new Lazy<IMeasurementFilterDiscoveryAgent>(
                () => new MeasurementFilterDiscoveryAgent(DiscoveryOptions, GetExportedTypes)
                , ExecutionAndPublication);
        }

        private IEnumerable<IPerformanceMeasurementDescriptor> _measurements;

        /// <summary>
        /// Gets the Measurements discovered by the service.
        /// </summary>
        public override IEnumerable<IPerformanceMeasurementDescriptor> Measurements => _measurements;

        private IEnumerable<IPerformanceMeasurementDescriptor> PrivateMeasurements
        {
            set { _measurements = (value ?? new List<IPerformanceMeasurementDescriptor>()).ToArray(); }
        }

        private void OnDiscoverperformanceMeasurementFilterDescriptors()
        {
            PrivateMeasurements = _lazyMeasurementFilterDiscoveryAgent.Value.ToArray();
        }

        /// <summary>
        /// Discover handler.
        /// </summary>
        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverperformanceMeasurementFilterDescriptors();
        }

        /// <summary>
        /// Returns an instance of <see cref="IInstallerContext"/>.
        /// </summary>
        /// <returns></returns>
        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(DiscoveryOptions, this);
        }
    }
}
