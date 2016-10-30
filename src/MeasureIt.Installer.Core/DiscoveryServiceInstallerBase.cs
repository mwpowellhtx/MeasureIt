using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;

namespace MeasureIt
{
    using Discovery;

    /// <summary>
    /// Provides <see cref="IInstrumentationDiscoveryService"/>
    /// <see cref="System.Configuration.Install.Installer"/> hooks. All you need to do is
    /// reference the appropriate set of <see cref="Assembly"/>(ies) and the discovery service.
    /// </summary>
    [Description("MeasureIt Discovery Service Installer")]
    public abstract class DiscoveryServiceInstallerBase : Installer, IDiscoveryServiceInstaller
    {
        private readonly IInstallerInstrumentationDiscoveryService _discoveryService;

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="discoveryService"></param>
        protected DiscoveryServiceInstallerBase(IInstallerInstrumentationDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            using (var context = _discoveryService.GetInstallerContext())
            {
                context.InstallAsync().Wait();
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            using (var context = _discoveryService.GetInstallerContext())
            {
                context.UninstallAsync().Wait();
            }

            base.Uninstall(savedState);
        }

        // TODO: TBD: run async with cancelation token...

    }
}
