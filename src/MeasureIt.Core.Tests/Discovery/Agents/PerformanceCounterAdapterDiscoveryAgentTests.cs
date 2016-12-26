using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    public class DefaultOptionsPerformanceCounterAdapterDiscoveryAgentTests
        : PerformanceCounterAdapterDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions().VerifyOptions();
        }

        public DefaultOptionsPerformanceCounterAdapterDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }

    public class IncludingNonPublicOptionsPerformanceCounterAdapterDiscoveryAgentTests
        : PerformanceCounterAdapterDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return new InstrumentationDiscoveryOptions {MethodBindingAttr = methodBindingAttr}.VerifyOptions(methodBindingAttr);
        }

        public IncludingNonPublicOptionsPerformanceCounterAdapterDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }
}