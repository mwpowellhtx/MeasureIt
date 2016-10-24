using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents the blueprint for <see cref="PerformanceCounterCategory"/>.
    /// </summary>
    public interface IPerformanceCounterCategoryAttribute : IAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterCategoryDescriptor Descriptor { get; }
    }
}
