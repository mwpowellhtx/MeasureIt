using System;

namespace MeasureIt
{
    /// <summary>
    /// Named <see cref="IDisposable"/> interface.
    /// </summary>
    public interface INamedDisposable : IDisposable
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        string Name { get; set; }
    }
}
