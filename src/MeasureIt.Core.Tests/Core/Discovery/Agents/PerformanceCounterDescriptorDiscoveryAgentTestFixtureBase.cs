using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public abstract class PerformanceCounterDescriptorDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceCounterDescriptorDiscoveryAgent
            , IPerformanceCounterDescriptor>
    {
        protected override PerformanceCounterDescriptorDiscoveryAgent CreateAgent(
            InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterDescriptorDiscoveryAgentTestFixtureBase(
            InstrumentationDiscovererOptions options)
            : base(options)
        {
        }

        protected virtual void VerifyMethods(IEnumerable<MethodInfo> methods)
        {
            Assert.NotNull(methods);
        }

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceCounterDescriptor> discoveredItems)
        {
            Assert.NotNull(discoveredItems);
            VerifyMethods(discoveredItems.Select(x => x.Method));
        }
    }
}
