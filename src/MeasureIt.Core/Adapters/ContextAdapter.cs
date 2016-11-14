using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Adapters
{
    /// <summary>
    /// Context Adapter base class.
    /// </summary>
    public abstract class ContextAdapter : Disposable, IContextAdapter
    {
        private readonly Lazy<IEnumerable<IPerformanceCounterCategoryAdapter>> _lazyCategoryAdapters;

        /// <summary>
        /// Gets the CategoryAdapters.
        /// </summary>
        protected IEnumerable<IPerformanceCounterCategoryAdapter> CategoryAdapters
        {
            get { return _lazyCategoryAdapters.Value; }
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="categoryAdapters"></param>
        protected ContextAdapter(
            IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
        {
            _lazyCategoryAdapters = new Lazy<IEnumerable<IPerformanceCounterCategoryAdapter>>(
                () => (categoryAdapters ?? new List<IPerformanceCounterCategoryAdapter>()).ToArray());
        }
    }
}
