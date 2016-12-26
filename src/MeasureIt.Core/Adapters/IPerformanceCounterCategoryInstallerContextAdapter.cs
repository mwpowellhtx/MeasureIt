using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt.Adapters
{
    using CategoryTuple = Tuple<IPerformanceCounterCategoryAdapter, PerformanceCounterCategory>;

    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterCategoryInstallerContextAdapter : IInstallerContextAdapter
    {
        /// <summary>
        /// Returns the Installed Categories corresponding with this <see cref="IContextAdapter"/>.
        /// </summary>
        IEnumerable<CategoryTuple> GetInstalledCategories();
    }
}
