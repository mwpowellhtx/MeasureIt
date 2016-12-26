using System.Collections.Generic;

namespace MeasureIt.Adapters
{
    /// <summary>
    /// Establishes an Installer Context Adapter base class.
    /// </summary>
    public abstract class InstallerContextAdapter : ContextAdapter, IInstallerContextAdapter
    {
        /// <summary>
        /// Protected Constructors
        /// </summary>
        /// <param name="categoryAdapters"></param>
        protected InstallerContextAdapter(
            IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
        }
    }

    /// <summary>
    /// Establishes an Uninstaller Context Adapter base class.
    /// </summary>
    public abstract class UninstallerContextAdapter : ContextAdapter, IUninstallerContextAdapter
    {
        /// <summary>
        /// Protected Constructors
        /// </summary>
        /// <param name="categoryAdapters"></param>
        protected UninstallerContextAdapter(
            IEnumerable<IPerformanceCounterCategoryAdapter> categoryAdapters
            )
            : base(categoryAdapters)
        {
        }
    }
}
