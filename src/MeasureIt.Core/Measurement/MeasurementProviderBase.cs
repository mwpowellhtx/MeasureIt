using System;
using System.Reflection;

namespace MeasureIt.Measurement
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MeasurementProviderBase : IMeasurementProvider
    {
        private readonly IRuntimeInstrumentationDiscoveryService _discoveryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryService"></param>
        protected MeasurementProviderBase(IRuntimeInstrumentationDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            return null;
        }
    }
}