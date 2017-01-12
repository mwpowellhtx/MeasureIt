using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MeasureIt.Adapters
{
    using Discovery;
    using static LazyThreadSafetyMode;
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
        /// <param name="discoveryOptions"></param>
        /// <param name="categoryAdapters"></param>
        internal PerformanceCounterCategoryUninstallerContextAdapter(
            IInstrumentationDiscoveryOptions discoveryOptions
            , IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
            var o = discoveryOptions;

            _lazyCategories = new Lazy<IEnumerable<CategoryTuple>>(
                () => CategoryAdapters.Select(a =>
                {
                    var name = a.Name.PrepareCategoryName(o);
                    var exists = PerformanceCounterCategory.Exists(name);
                    if (exists) PerformanceCounterCategory.Delete(name);
                    return Tuple.Create(a, exists);
                }), ExecutionAndPublication);
        }
    }
}
