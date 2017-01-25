namespace MeasureIt
{
    using Discovery;

    /// <summary>
    /// Instrumentation discovery service for web purposes.
    /// </summary>
    public interface IMvcMeasurePerformanceAttribute
        : IMeasurePerformanceAttribute<
            IMvcPerformanceMeasurementDescriptor
        >
    {
        /// <summary>
        /// Gets or sets the Start and Stop Boundary <see cref="MeasurementBoundary"/>.
        /// </summary>
        MeasurementBoundary[] Boundary { get; set; }

        /// <summary>
        /// Gets the Start <see cref="MeasurementBoundary"/>.
        /// </summary>
        MeasurementBoundary StartBoundary { get; }

        /// <summary>
        /// Gets the Stop <see cref="MeasurementBoundary"/>.
        /// </summary>
        MeasurementBoundary StopBoundary { get; }
    }
}
