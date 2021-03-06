﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MeasureIt.Discovery
{
    using Agents;
    using Contexts;
    using static LazyThreadSafetyMode;

    // TODO: TBD: copy (or inherit) from this one extending into WebApi (and later, Mvc)...
    /// <summary>
    /// Instrumentation discovery service for web purposes.
    /// </summary>
    public class HttpActionInstrumentationDiscoveryService : RuntimeInstrumentationDiscoveryService
        , IHttpActionInstrumentationDiscoveryService
    {
        private readonly Lazy<IMeasurementFilterDiscoveryAgent> _lazyMeasurementFilterDiscoveryAgent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryOptions"></param>
        public HttpActionInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions discoveryOptions)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(DiscoveryOptions, this);
        }
    }
}
