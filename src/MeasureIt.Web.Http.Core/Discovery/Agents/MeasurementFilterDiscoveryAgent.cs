namespace MeasureIt.Discovery.Agents
{
    using Web.Http.Filters;

    /// <summary>
    /// 
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
