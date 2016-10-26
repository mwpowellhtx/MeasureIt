using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class InstallerDiscoveryTestFixtureBase<TService> : DiscoveryServiceTestFixtureBase<TService>
        where TService : InstallerInstrumentationDiscoveryService
    {
        protected InstallerDiscoveryTestFixtureBase(IInstrumentationDiscoveryOptions options,
            IEnumerable<Assembly> assemblies, ServiceFactoryDelegate serviceFactory)
            : base(options, assemblies, serviceFactory)
        {
        }

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

            Assert.NotNull(service.CounterDescriptors);
            Assert.NotEmpty(service.CounterDescriptors);

            OnVerifyCounterDescriptors(service.CounterDescriptors);
        }

        protected abstract void OnVerifyCategoryDescriptors(
            IEnumerable<IPerformanceCounterCategoryDescriptor> descriptors);

        protected abstract void OnVerifyCounterDescriptors(
            IEnumerable<IPerformanceCounterDescriptor> descriptors);
    }
}
