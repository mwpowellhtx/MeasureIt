using System;
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
