using System;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Integration.Installer
{
    using Discovery;

    public class IntegrationDiscoveryServiceInstaller : DiscoveryServiceInstallerBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            return new InstrumentationDiscovererOptions();
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IDescriptor).Assembly;
            yield return typeof(Support.Root).Assembly;
        }

        private static IInstallerInstrumentationDiscoveryService GetService()
        {
            return new InstallerInstrumentationDiscoveryService(GetOptions(), GetAssemblies());
        }

        private static readonly Lazy<IInstallerInstrumentationDiscoveryService> LazyService
            = new Lazy<IInstallerInstrumentationDiscoveryService>(GetService);

        public IntegrationDiscoveryServiceInstaller()
            : base(LazyService.Value)
        {
        }
    }
}
