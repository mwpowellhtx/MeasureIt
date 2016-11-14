using System.Linq;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class InstallPerformanceCounterCategoriesTests
        : InstallerDiscoveryServicePublicInstanceMembersTests
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions
            {
                Assemblies = GetAssemblies(),
                ThrowOnInstallerFailure = false
            }.VerifyOptions();
        }

        private readonly IInstrumentationDiscoveryOptions _options;

        protected override IInstrumentationDiscoveryOptions Options
        {
            get { return _options; }
        }

        public InstallPerformanceCounterCategoriesTests()
        {
            _options = GetOptions();
        }

        protected virtual void OnInstall()
        {
            using (var context = DiscoveryService.GetInstallerContext())
            {
                context.Install();
            }
        }

        protected virtual void OnUninstall()
        {
            // TODO: TBD: may re-factor "getdiscovereddiscoveryservice" in terms of lazily initialized discoveryservice property?
            using (var context = DiscoveryService.GetInstallerContext())
            {
                context.Uninstall();
            }
        }

        [Fact]
        public void CanDiscoveryServiceDiscoverAndInstall()
        {
            /* TODO: TBD: okay, so the "problem" here is the fact that one or all discovery services
             * need; started connecting measurement descriptors with the category adapters, still a work in progress . */
            OnInstall();

            var counters = DiscoveryService.Measurements.SelectMany(
                d => d.Adapters.Select(a => a.Counters)).ToArray();

            Assert.All(counters, x => Assert.NotNull(x));
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                OnUninstall();
            }

            base.Dispose(disposing);
        }
    }
}
