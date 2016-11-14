using System.Collections.Generic;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;

    public abstract class PerformanceCounterAdapterDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceCounterAdapterDiscoveryAgent
            , IPerformanceCounterAdapter>
    {
        protected override PerformanceCounterAdapterDiscoveryAgent CreateAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterAdapterDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterAdapterDiscoveryAgentTestFixtureBase(
            IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        protected sealed override void OnItemsDiscovered(IEnumerable<IPerformanceCounterAdapter> discoveredItems)
        {
            discoveredItems.Verify();
        }
    }
}
