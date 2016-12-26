using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents the blueprint for <see cref="CounterCreationData"/>.
    /// </summary>
    public interface ICounterCreationDataAttribute : IAttribute
    {
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        ICounterCreationDataDescriptor Descriptor { get; }
    }
}
