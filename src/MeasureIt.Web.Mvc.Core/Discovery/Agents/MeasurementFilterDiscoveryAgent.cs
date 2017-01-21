namespace MeasureIt.Discovery.Agents
{
    using Web.Mvc.Filters;

    /// <summary>
    /// Measurement filter discovery agent.
    /// </summary>
    public class MeasurementFilterDiscoveryAgent
        : PerformanceMeasurementDescriptorDiscoveryAgentBase<
            PerformanceMeasurementFilterAttribute>, IMeasurementFilterDiscoveryAgent
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
