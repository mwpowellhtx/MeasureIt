using System;
using System.Collections.Generic;

namespace MeasureIt.Adapters
{
    using CategoryTuple = Tuple<IPerformanceCounterCategoryAdapter, bool>;

    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterCategoryUninstallerContextAdapter : IUninstallerContextAdapter
    {
        /// <summary>
        /// Tries to Uninstall the Categories.
        /// </summary>
        /// <param name="categoryNames"></param>
        /// <returns></returns>
        IEnumerable<CategoryTuple> TryUninstallCategories(out IEnumerable<string> categoryNames);
    }
}
