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

            Assert.NotNull(service.Measurements);
            Assert.Empty(service.Measurements);
        }

        protected override void OnAfterDiscovery(TService service)
        {
            base.OnAfterDiscovery(service);

            Assert.NotNull(service.Measurements);
            Assert.NotEmpty(service.Measurements);
        }

        protected sealed override void OnVerifyDescriptors(TService service)
        {
            base.OnVerifyDescriptors(service);

            VerifyDiscoveredCounterDescriptors(service.Measurements);
        }

        protected virtual void VerifyDiscoveredCounterDescriptors(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            Assert.NotNull(descriptors);
            Assert.NotEmpty(descriptors);
        }
    }
}
