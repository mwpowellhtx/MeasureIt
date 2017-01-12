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
        /// <param name="discoveryOptions"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions discoveryOptions)
            : base(discoveryOptions)
        {
        }

        /// <summary>
        /// Returns an Installer Context for use from the Discovery Service.
        /// </summary>
        /// <returns></returns>
        // TODO: TBD: I'm not sure we need this method quite as much as simple leveraging the Install/TryUninstall extension methods...
        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(DiscoveryOptions, this);
        }
    }
}
