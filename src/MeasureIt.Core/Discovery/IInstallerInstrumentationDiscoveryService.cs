using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    using Contexts;

    /// <summary>
    /// 
    /// </summary>
    public interface IInstallerInstrumentationDiscoveryService : IRuntimeInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IPerformanceCounterCategoryDescriptor> CategoryDescriptors { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IInstallerContext GetInstallerContext();
    }
}
