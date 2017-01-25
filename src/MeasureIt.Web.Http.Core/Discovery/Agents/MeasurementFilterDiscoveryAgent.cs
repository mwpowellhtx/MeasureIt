namespace MeasureIt.Discovery.Agents
{
    using Web.Http.Filters;

    /// <summary>
    /// Measurement filter discovery agent regarding
    /// <see cref="IPerformanceMeasurementDescriptor"/> and
    /// <see cref="PerformanceMeasurementFilterAttribute"/> concerns.
    /// </summary>
    public class MeasurementFilterDiscoveryAgent
        : PerformanceMeasurementDescriptorDiscoveryAgentBase<
                IPerformanceMeasurementDescriptor
                , PerformanceMeasurementFilterAttribute
            >
            , IMeasurementFilterDiscoveryAgent
    {
        internal MeasurementFilterDiscoveryAgent(
            IInstrumentationDiscoveryOptions options
            , DiscoveryServiceExportedTypesGetterDelegate getExportedTypes
        )
            : base(options, getExportedTypes)
        {
        }
    }
}
