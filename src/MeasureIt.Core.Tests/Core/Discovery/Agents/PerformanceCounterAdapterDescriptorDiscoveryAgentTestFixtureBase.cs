using System.Collections.Generic;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;

    public abstract class PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceCounterAdapterDescriptorDiscoveryAgent
            , IPerformanceCounterAdapterDescriptor>
    {
        protected override PerformanceCounterAdapterDescriptorDiscoveryAgent CreateAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterAdapterDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase(
            IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        protected sealed override void OnItemsDiscovered(IEnumerable<IPerformanceCounterAdapterDescriptor> discoveredItems)
        {
            discoveredItems.Verify();
        }
    }
}
