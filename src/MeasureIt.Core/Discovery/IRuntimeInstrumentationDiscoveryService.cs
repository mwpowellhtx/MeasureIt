using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRuntimeInstrumentationDiscoveryService : IInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IPerformanceCounterDescriptor> CounterDescriptors { get; }
    }
}
