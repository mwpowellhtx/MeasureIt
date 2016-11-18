using System;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Contexts;

    /// <summary>
    /// 
    /// </summary>
    public interface IRuntimeInstrumentationDiscoveryService : IInstrumentationDiscoveryService
    {
        /// <summary>
        /// Gets the CategoryAdapters associated with the Installer.
        /// </summary>
        IDictionary<Type, IPerformanceCounterCategoryAdapter> CategoryAdapters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo methodInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMeasurementContext GetMeasurementContext();
    }
}
