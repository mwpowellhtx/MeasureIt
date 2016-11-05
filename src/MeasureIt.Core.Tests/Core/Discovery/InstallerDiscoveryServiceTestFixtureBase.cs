using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class InstallerDiscoveryServiceTestFixtureBase<TService>
        : DiscoveryServiceTestFixtureBase<TService>
        where TService : InstallerInstrumentationDiscoveryService
    {
        protected override void OnBeforeDiscovery(TService service)
        {
            base.OnBeforeDiscovery(service);

            Assert.NotNull(service.CategoryDescriptors);
            Assert.Empty(service.CategoryDescriptors);
        }

        protected override void OnAfterDiscovery(TService service)
        {
            base.OnAfterDiscovery(service);

            Assert.NotNull(service.CategoryDescriptors);
            Assert.NotEmpty(service.CategoryDescriptors);

            OnVerifyCategoryDescriptors(service.CategoryDescriptors);

            Assert.NotNull(service.MeasurementDescriptors);
            Assert.NotEmpty(service.MeasurementDescriptors);

            OnVerifyCounterDescriptors(service.MeasurementDescriptors);
        }

        protected abstract void OnVerifyCategoryDescriptors(
            IEnumerable<IPerformanceCounterCategoryDescriptor> descriptors);

        protected abstract void OnVerifyCounterDescriptors(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors);
    }
}
