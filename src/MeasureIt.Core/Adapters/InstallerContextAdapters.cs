using System;
using System.Collections.Generic;

namespace MeasureIt.Adapters
{
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
        internal static string PrepareCategoryName(this string name, bool prepareName = true)
        {
            /* Maximum length of a Performance Counter Category name, and perhaps other naming conventions...
             * http://msdn.microsoft.com/en-us/library/sb32hxtc.aspx (PerformanceCounterCategory.Create)  */

            // We will want to trim it in either case.
            name = name.Trim();

            const int maxLength = 80;

            // Trim one final time in the event that the substring has a trailing space.
            return prepareName ? name.Substring(0, Min(name.Length, maxLength)).Trim() : name;
        }
    }
}
