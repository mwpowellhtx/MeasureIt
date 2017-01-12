using System;
using System.Collections.Generic;

namespace MeasureIt.Adapters
{
    using Discovery;
    using static Math;

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

    internal static class ContextExtensionMethods
    {
        internal static string PrepareCategoryName(this string name, IInstrumentationDiscoveryOptions options)
        {
            const int maxLength = 80;
            return options.PrepareCategoryName
                ? name.Trim().Substring(0, Min(name.Trim().Length, maxLength))
                : name;
        }
    }
}
