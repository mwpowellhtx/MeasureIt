using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public abstract class PerformanceMeasurementDescriptorDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceMeasurementDescriptorDiscoveryAgent
            , IPerformanceMeasurementDescriptor>
    {
        protected override PerformanceMeasurementDescriptorDiscoveryAgent CreateAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceMeasurementDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceMeasurementDescriptorDiscoveryAgentTestFixtureBase(
            IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        protected virtual void VerifyMethods(IEnumerable<MethodInfo> methods)
        {
            Assert.NotNull(methods);
        }

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceMeasurementDescriptor> discoveredItems)
        {
            Assert.NotNull(discoveredItems);
            VerifyMethods(discoveredItems.Select(x => x.Method));
        }
    }
}
