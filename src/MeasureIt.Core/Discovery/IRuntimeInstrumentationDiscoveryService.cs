using System;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Contexts;

    /// <summary>
    /// Discovery service for purposes of supporting Runtime Instrumentation.
    /// </summary>
    public interface IRuntimeInstrumentationDiscoveryService : IInstrumentationDiscoveryService
    {
        /// <summary>
        /// Gets the CategoryAdapters associated with the Installer.
        /// </summary>
        IDictionary<Type, IPerformanceCounterCategoryAdapter> CategoryAdapters { get; }
    }
}
