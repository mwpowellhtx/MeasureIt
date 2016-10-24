using System.Reflection;

namespace MeasureIt.Measurement
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementProvider
    {
        // TODO: TBD: do we need any other arguments?
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        IMeasurementContext GetMeasurementContext(MethodInfo method);
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class MeasurementProvider : IMeasurementProvider
    {
        private readonly IRuntimeInstrumentationDiscoveryService _discoveryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryService"></param>
        public MeasurementProvider(IRuntimeInstrumentationDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        public IMeasurementContext GetMeasurementContext(MethodInfo method)
        {

            //_discoveryService.CounterDescriptors.
            throw new System.NotImplementedException();
        }
    }
}
