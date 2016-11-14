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

            Assert.NotNull(service.CategoryAdapters);
            Assert.Empty(service.CategoryAdapters);
        }

        protected override void OnAfterDiscovery(TService service)
        {
            base.OnAfterDiscovery(service);

            Assert.NotNull(service.CategoryAdapters);
            Assert.NotEmpty(service.CategoryAdapters);

            OnVerifyCategoryAdapters(service.CategoryAdapters);

            Assert.NotNull(service.Measurements);
            Assert.NotEmpty(service.Measurements);

            OnVerifyPerformanceMeasurements(service.Measurements);
        }

        protected abstract void OnVerifyCategoryAdapters(
            IEnumerable<IPerformanceCounterCategoryAdapter> categories);

        protected abstract void OnVerifyPerformanceMeasurements(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors);
    }
}
