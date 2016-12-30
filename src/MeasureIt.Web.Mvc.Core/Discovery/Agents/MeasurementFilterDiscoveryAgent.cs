namespace MeasureIt.Web.Mvc.Discovery.Agents
{
    using Filters;
    using MeasureIt.Discovery;
    using MeasureIt.Discovery.Agents;

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
