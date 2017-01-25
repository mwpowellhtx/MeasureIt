using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Base interface for the <see cref="IMeasurePerformanceAttribute"/> tree.
    /// </summary>
    public interface IMeasurePerformanceAttributeBase : IAttribute
    {
    }

    /// <summary>
    /// Represents the blueprint for <see cref="PerformanceCounter"/>.
    /// </summary>
    /// <typeparam name="TDescriptor"></typeparam>
    public interface IMeasurePerformanceAttribute<out TDescriptor> : IMeasurePerformanceAttributeBase
        where TDescriptor : class, IPerformanceMeasurementDescriptor
    {
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        TDescriptor Descriptor { get; }
    }

    /// <summary>
    /// Represents the Descriptor concerns for <see cref="IPerformanceMeasurementDescriptor"/>.
    /// </summary>
    public interface IMeasurePerformanceAttribute : IMeasurePerformanceAttribute<IPerformanceMeasurementDescriptor>
    {
    }
}
