using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents the blueprint for <see cref="PerformanceCounter"/>.
    /// </summary>
    public interface IPerformanceCounterAttribute : IAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterDescriptor Descriptor { get; }
    }
}
