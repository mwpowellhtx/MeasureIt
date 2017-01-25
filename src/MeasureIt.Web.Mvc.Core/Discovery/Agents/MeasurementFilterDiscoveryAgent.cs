namespace MeasureIt.Discovery.Agents
{
    using Web.Mvc.Filters;

    /// <summary>
    /// Measurement filter discovery agent regarding
    /// <see cref="IMvcPerformanceMeasurementDescriptor"/> and
    /// <see cref="PerformanceMeasurementFilterAttribute"/> concerns.
    /// </summary>
    public class MeasurementFilterDiscoveryAgent
        : PerformanceMeasurementDescriptorDiscoveryAgentBase<
                IMvcPerformanceMeasurementDescriptor
                , PerformanceMeasurementFilterAttribute>
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
