using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Category Adapter is the concrete type that facilitates the feature set for each
    /// <see cref="PerformanceCounterCategory"/>. <see cref="IPerformanceCounterCategoryAdapter"/>
    /// should not be confused with <see cref="IPerformanceCounterCategoryDescriptor"/>.
    /// </summary>
    public interface IPerformanceCounterCategoryAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterCategoryDescriptor Descriptor { get; }
    }
}
