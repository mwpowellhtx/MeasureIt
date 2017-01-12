using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MeasureIt.Adapters
{
    using Discovery;
    using CategoryTuple = Tuple<IPerformanceCounterCategoryAdapter, PerformanceCounterCategory>;

    /// <summary>
    /// Context adapter for Performance Counter category creation.
    /// </summary>
    public class PerformanceCounterCategoryInstallerContextAdapter
        : InstallerContextAdapter
            , IPerformanceCounterCategoryInstallerContextAdapter
    {
        private readonly Lazy<IEnumerable<CategoryTuple>> _lazyCategories;

        /// <summary>
        /// Returns the installed <see cref="CategoryTuple"/> items.
        /// </summary>
        /// <returns></returns>
        /// <see cref="PerformanceCounterCategory.Create(string,string,PerformanceCounterCategoryType,CounterCreationDataCollection)"/>
        public virtual IEnumerable<CategoryTuple> GetInstalledCategories()
        {
            return _lazyCategories.Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <param name="categoryAdapters"></param>
        internal PerformanceCounterCategoryInstallerContextAdapter(
            IInstrumentationDiscoveryOptions discoveryOptions
            , IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            var o = discoveryOptions;

            _lazyCategories = new Lazy<IEnumerable<CategoryTuple>>(
                () => CategoryAdapters.Select(a =>
                {
                    // No fancy naming convensions here, joining paths, etc, just rely on the individual descriptor Names.
                    var items = a.CreationData.Select(x => new CounterCreationData(x.Name, x.Help, x.CounterType)).ToArray();
                    var data = new CounterCreationDataCollection(items);

                    var name = a.Name.PrepareCategoryName(o);

                    return Tuple.Create(a, PerformanceCounterCategory.Exists(name)
                        ? null
                        : PerformanceCounterCategory.Create(name, a.Help, a.CategoryType, data));
                }), execAndPubThreadSafety);
        }
    }
}
