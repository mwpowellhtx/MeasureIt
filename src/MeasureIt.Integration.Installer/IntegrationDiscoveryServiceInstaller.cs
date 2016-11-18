using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Integration.Installer
{
    using Discovery;

    public class IntegrationDiscoveryServiceInstaller : DiscoveryServiceInstallerBase
    {
        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IDescriptor).Assembly;
            yield return typeof(Support.Root).Assembly;
        }

        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions {Assemblies = GetAssemblies()};
        }

        private static IInstallerInstrumentationDiscoveryService GetService()
        {
            return new InstallerInstrumentationDiscoveryService(GetOptions());
        }

        /// <summary>
        /// <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/>
        /// </summary>
        private const LazyThreadSafetyMode ExecAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

        private static readonly Lazy<IInstallerInstrumentationDiscoveryService> LazyService
            = new Lazy<IInstallerInstrumentationDiscoveryService>(GetService, ExecAndPubThreadSafety);

        public IntegrationDiscoveryServiceInstaller()
            : base(LazyService.Value)
        {
        }
    }
}
