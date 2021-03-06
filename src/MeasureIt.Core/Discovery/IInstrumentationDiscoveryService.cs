﻿using System;
using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// InstrumentationDiscoveryService interaface.
    /// </summary>
    public interface IInstrumentationDiscoveryService
    {
        /// <summary>
        /// Discovered event.
        /// </summary>
        event EventHandler<EventArgs> Discovered;

        /// <summary>
        /// Discovers the Performance Monitoring Instrumentation from the Assemblies.
        /// </summary>
        void Discover();

        // TODO: TBD: may want to setup a "proper" enum value
        /// <summary>
        /// 
        /// </summary>
        bool IsPending { get; }

        /// <summary>
        /// Gets the Measurements.
        /// </summary>
        IEnumerable<IPerformanceMeasurementDescriptor> Measurements { get; }
    }
}
