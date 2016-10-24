using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public abstract class PerformanceCounterCategoryDescriptorDiscoveryAgentTestFixtureBase
        : DiscoveryAgentTestFixtureBase<PerformanceCounterCategoryDescriptorDiscoveryAgent
            , IPerformanceCounterCategoryDescriptor>
    {
        protected override PerformanceCounterCategoryDescriptorDiscoveryAgent CreateAgent(
            InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterCategoryDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterCategoryDescriptorDiscoveryAgentTestFixtureBase(
            InstrumentationDiscovererOptions options)
            : base(options)
        {
        }

        protected static IEnumerable<IPerformanceCounterCategoryDescriptor> ApplyOrdering(
            IEnumerable<IPerformanceCounterCategoryDescriptor> discoveredItems)
        {
            Assert.NotNull(discoveredItems);

            return discoveredItems.OrderBy(x => x.Name);
        }

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceCounterCategoryDescriptor> discoveredItems)
        {
            var orderedItems = ApplyOrdering(discoveredItems);

            Assert.Collection(orderedItems,
                d =>
                {
                    Assert.NotNull(d);
                    d.Name.CanParse<string, Guid>(Guid.TryParse);
                    Assert.Equal(string.Empty, d.Help);
                    Assert.Equal(PerformanceCounterCategoryType.MultiInstance, d.CategoryType);
                }
                );
        }
    }
}
