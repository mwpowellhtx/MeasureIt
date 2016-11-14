using System;

namespace MeasureIt
{
    /// <summary>
    /// Establishes a common Descriptor theme for the Assembly.
    /// </summary>
    public interface IDescriptor : ICloneable
    {
        /// <summary>
        /// Gets the Id.
        /// </summary>
        Guid Id { get; }
    }
}
