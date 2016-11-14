using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt.Adapters
{
    using CategoryTuple = Tuple<IPerformanceCounterCategoryAdapter, bool>;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterCategoryUninstallerContextAdapter
        : UninstallerContextAdapter
            , IPerformanceCounterCategoryUninstallerContextAdapter
    {
        private readonly Lazy<IEnumerable<CategoryTuple>> _lazyCategories;

        /// <summary>
        /// Returns the 
        /// </summary>
        /// <param name="categoryNames"></param>
        /// <returns></returns>
        public virtual IEnumerable<CategoryTuple> TryUninstallCategories(out IEnumerable<string> categoryNames)
        {
            var uninstalled = _lazyCategories.Value.ToArray();
            categoryNames = uninstalled.Select(c => c.Item1.Name).ToArray();
            return uninstalled;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryAdapters"></param>
        internal PerformanceCounterCategoryUninstallerContextAdapter(
            IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
            _lazyCategories = new Lazy<IEnumerable<CategoryTuple>>(
                () => CategoryAdapters.Select(a =>
                {
                    var exists = PerformanceCounterCategory.Exists(a.Name);
                    if (exists) PerformanceCounterCategory.Delete(a.Name);
                    return Tuple.Create(a, exists);
                }));
        }
    }
}
