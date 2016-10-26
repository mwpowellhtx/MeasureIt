﻿using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    public class DefaultOptionsPerformanceCounterAdapterDiscoveryAgentTests
        : PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
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
        : PerformanceCounterAdapterDescriptorDiscoveryAgentTestFixtureBase
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