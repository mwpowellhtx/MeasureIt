namespace MeasureIt.Discovery.Agents
{
    public class DefaultOptionsPerformanceCounterCategoryDiscoveryAgentTests
        : PerformanceCounterCategoryDescriptorDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions().VerifyOptions();
        }

        public DefaultOptionsPerformanceCounterCategoryDiscoveryAgentTests()
            : base(GetOptions())
        {
        }
    }
}