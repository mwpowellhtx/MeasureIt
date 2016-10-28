using System;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Measurement;

    /// <summary>
    /// 
    /// </summary>
    public interface IRuntimeInstrumentationDiscoveryService : IInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IMeasurePerformanceDescriptor> CounterDescriptors { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo methodInfo);
    }
}
