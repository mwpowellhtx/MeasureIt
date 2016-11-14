using System.Collections.Generic;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterAdapterDiscoveryAgent
        : IDiscoveryAgent<IPerformanceCounterAdapter>
    {
        /// <summary>
        /// Gets the Adapters.
        /// </summary>
        IEnumerable<IPerformanceCounterAdapter> Adapters { get; }
    }
}
