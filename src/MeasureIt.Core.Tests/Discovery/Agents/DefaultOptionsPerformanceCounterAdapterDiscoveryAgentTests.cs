using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Xunit;

    public class DefaultOptionsPerformanceCounterAdapterDiscoveryAgentTests
        : PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions();
            Assert.NotNull(options);
            Assert.Equal(methodBindingAttr, options.MethodBindingAttr);
            return options;
        }

        public DefaultOptionsPerformanceCounterAdapterDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }

    public class IncludingNonPublicOptionsPerformanceCounterAdapterDiscoveryAgentTests
        : PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions {MethodBindingAttr = methodBindingAttr};
            Assert.NotNull(options);
            Assert.Equal(methodBindingAttr, options.MethodBindingAttr);
            return options;
        }

        public IncludingNonPublicOptionsPerformanceCounterAdapterDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }
}