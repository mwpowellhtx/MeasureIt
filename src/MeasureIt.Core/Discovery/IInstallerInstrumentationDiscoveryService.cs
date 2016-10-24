using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstallerInstrumentationDiscoveryService : IRuntimeInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IPerformanceCounterCategoryDescriptor> CategoryDescriptors { get; }
    }
}
