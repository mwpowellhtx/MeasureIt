using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public abstract class PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceCounterAdapterDescriptorDiscoveryAgent
            , IPerformanceCounterAdapterDescriptor>
    {
        protected override PerformanceCounterAdapterDescriptorDiscoveryAgent CreateAgent(
            InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterAdapterDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase(
            InstrumentationDiscovererOptions options)
            : base(options)
        {
        }

        protected sealed override void OnItemsDiscovered(IEnumerable<IPerformanceCounterAdapterDescriptor> discoveredItems)
        {
            discoveredItems.Verify();
        }
    }
}
