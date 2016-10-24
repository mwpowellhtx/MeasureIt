using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class RuntimeDiscoveryTestFixtureBase<TService> : DiscoveryServiceTestFixtureBase<TService>
        where TService : RuntimeInstrumentationDiscoveryService
    {
        protected RuntimeDiscoveryTestFixtureBase(InstrumentationDiscovererOptions options,
            IEnumerable<Assembly> assemblies, ServiceFactoryDelegate serviceFactory)
            : base(options, assemblies, serviceFactory)
        {
        }

        protected override void OnBeforeDiscovery(TService service)
        {
            base.OnBeforeDiscovery(service);

            Assert.NotNull(service.CounterDescriptors);
            Assert.Empty(service.CounterDescriptors);

            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.Empty(service.CounterAdapterDescriptors);
        }

        protected override void OnAfterDiscovery(TService service)
        {
            base.OnAfterDiscovery(service);

            Assert.NotNull(service.CounterDescriptors);
            Assert.NotEmpty(service.CounterDescriptors);
        }

        protected sealed override void OnVerifyDescriptors(TService service)
        {
            base.OnVerifyDescriptors(service);

            VerifyDiscoveredCounterDescriptors(service.CounterDescriptors);
        }

        protected virtual void VerifyDiscoveredCounterDescriptors(
            IEnumerable<IPerformanceCounterDescriptor> descriptors)
        {
            Assert.NotNull(descriptors);
            Assert.NotEmpty(descriptors);
        }
    }
}
