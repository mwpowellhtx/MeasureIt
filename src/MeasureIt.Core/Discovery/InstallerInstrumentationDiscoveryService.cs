using System.Linq;

namespace MeasureIt.Discovery
{
    using Contexts;

    /// <summary>
    /// Installer requires the Runtime Discovery Service as well as connecting the dots.
    /// </summary>
    public class InstallerInstrumentationDiscoveryService : RuntimeInstrumentationDiscoveryService
        , IInstallerInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        // TODO: TBD: I'm not sure we need this method quite as much as simple leveraging the Install/TryUninstall extension methods...
        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(Options, this);
        }
    }
}
