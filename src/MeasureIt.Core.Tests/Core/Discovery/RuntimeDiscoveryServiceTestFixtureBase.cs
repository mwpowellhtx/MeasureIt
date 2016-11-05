using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class RuntimeDiscoveryServiceTestFixtureBase<TService>
        : DiscoveryServiceTestFixtureBase<TService>
        where TService : RuntimeInstrumentationDiscoveryService
    {
        protected override void OnBeforeDiscovery(TService service)
        {
            base.OnBeforeDiscovery(service);

            Assert.NotNull(service.MeasurementDescriptors);
            Assert.Empty(service.MeasurementDescriptors);

            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.Empty(service.CounterAdapterDescriptors);
        }

        protected override void OnAfterDiscovery(TService service)
        {
            base.OnAfterDiscovery(service);

            Assert.NotNull(service.MeasurementDescriptors);
            Assert.NotEmpty(service.MeasurementDescriptors);
        }

        protected sealed override void OnVerifyDescriptors(TService service)
        {
            base.OnVerifyDescriptors(service);

            VerifyDiscoveredCounterDescriptors(service.MeasurementDescriptors);
        }

        protected virtual void VerifyDiscoveredCounterDescriptors(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            Assert.NotNull(descriptors);
            Assert.NotEmpty(descriptors);
        }
    }
}
