namespace MeasureIt
{
    using Discovery;

    /// <summary>
    /// Establishes a <see cref="IPerformanceMeasurementDescriptor"/> including
    /// <see cref="MeasurementBoundary"/> pair.
    /// </summary>
    public interface IMvcPerformanceMeasurementDescriptor : IPerformanceMeasurementDescriptor
    {
        /// <summary>
        /// Gets the Boundary <see cref="MeasurementBoundaryPair"/>.
        /// </summary>
        MeasurementBoundaryPair Boundary { get; }
    }
}
