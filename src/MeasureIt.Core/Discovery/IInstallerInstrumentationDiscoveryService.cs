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
        /// Gets the CategoryAdapters associated with the Installer.
        /// </summary>
        IEnumerable<IPerformanceCounterCategoryAdapter> CategoryAdapters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IInstallerContext GetInstallerContext();
    }
}
