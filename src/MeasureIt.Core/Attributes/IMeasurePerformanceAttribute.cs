using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents the blueprint for <see cref="PerformanceCounter"/>.
    /// </summary>
    public interface IMeasurePerformanceAttribute : IAttribute
    {
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        IPerformanceMeasurementDescriptor Descriptor { get; }
    }
}
