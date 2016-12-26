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
            IInstrumentationDiscoveryOptions options
            , DiscoveryServiceExportedTypesGetterDelegate getExportedTypes
            )
            : base(options, getExportedTypes)
        {
        }
    }
}
