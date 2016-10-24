using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Xunit;

    public class DefaultOptionsPerformanceCounterCategoryDiscoveryAgentTests
        : PerformanceCounterCategoryDescriptorDiscoveryAgentTestFixtureBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions();
            Assert.Equal(options.MethodBindingAttr, methodBindingAttr);
            return options;
        }

        public DefaultOptionsPerformanceCounterCategoryDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }
}