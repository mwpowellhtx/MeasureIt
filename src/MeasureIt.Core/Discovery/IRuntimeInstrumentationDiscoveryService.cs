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
        /// Gets the MeasurementDescriptors.
        /// </summary>
        IEnumerable<IPerformanceMeasurementDescriptor> MeasurementDescriptors { get; }

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
