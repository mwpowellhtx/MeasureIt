namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptorDiscoveryAgent
        : PerformanceMeasurementDescriptorDiscoveryAgentBase<
            MeasurePerformanceAttribute>
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
