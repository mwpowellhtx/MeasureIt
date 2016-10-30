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
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            return new PerformanceCounterCategoryDescriptorDiscoveryAgent(options, getExportedTypes);
        }

        protected PerformanceCounterCategoryDescriptorDiscoveryAgentTestFixtureBase(
            IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        protected override void OnItemsDiscovered(
            IEnumerable<IPerformanceCounterCategoryDescriptor> discoveredItems)
        {
            var orderedItems = discoveredItems.Order().ToArray();

            /* This is all the more we can verify at this level.
             * More is definitely expected from an integration test perspective, however. */

            Assert.Collection(orderedItems,
                d =>
                {
                    d.Name.CanParse<string, Guid>(Guid.TryParse);
                    Assert.Equal(string.Empty, d.Help);
                    Assert.Equal(PerformanceCounterCategoryType.MultiInstance, d.CategoryType);
                    d.Type.Confirm<DefaultPerformanceCounterCategoryAdapter>();
                }
                );
        }
    }
}
