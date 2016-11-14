using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt.Adapters
{
    using CategoryTuple = Tuple<IPerformanceCounterCategoryAdapter, PerformanceCounterCategory>;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterCategoryInstallerContextAdapter
        : InstallerContextAdapter
            , IPerformanceCounterCategoryInstallerContextAdapter
    {
        private readonly Lazy<IEnumerable<CategoryTuple>> _lazyCategories;

        public virtual IEnumerable<CategoryTuple> GetInstalledCategories()
        {
            return _lazyCategories.Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryAdapters"></param>
        internal PerformanceCounterCategoryInstallerContextAdapter(
            IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
            _lazyCategories = new Lazy<IEnumerable<CategoryTuple>>(
                () => CategoryAdapters.Select(a =>
                {
                    // No fancy naming convensions here, joining paths, etc, just rely on the individual descriptor Names.
                    var items = a.CreationData.Select(x => new CounterCreationData(x.Name, x.Help, x.CounterType)).ToArray();
                    var data = new CounterCreationDataCollection(items);

#if DEBUG
                    var names = items.Select(x => x.CounterName).ToArray();
#endif

                    return Tuple.Create(a, PerformanceCounterCategory.Exists(a.Name)
                        ? null
                        : PerformanceCounterCategory.Create(a.Name, a.Help, a.CategoryType, data));
                }));
        }
    }
}
