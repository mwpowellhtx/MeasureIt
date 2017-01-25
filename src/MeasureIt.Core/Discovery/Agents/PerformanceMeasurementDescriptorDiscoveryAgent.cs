namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// Discovery agent regarding the <see cref="IPerformanceMeasurementDescriptor"/> and
    /// <see cref="MeasurePerformanceAttribute"/> concerns.
    /// </summary>
    public class PerformanceMeasurementDescriptorDiscoveryAgent
        : PerformanceMeasurementDescriptorDiscoveryAgentBase<
            IPerformanceMeasurementDescriptor, MeasurePerformanceAttribute>
    {
        internal PerformanceMeasurementDescriptorDiscoveryAgent(
            IInstrumentationDiscoveryOptions discoveryOptions
            , DiscoveryServiceExportedTypesGetterDelegate getExportedTypes
        )
            : base(discoveryOptions, getExportedTypes)
        {
        }
    }
}
