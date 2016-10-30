using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public class InstallerContext : ContextBase, IInstallerContext
    {
        private readonly Lazy<IInstallerInstrumentationDiscoveryService> _lazyDiscoveryService;

        private IInstallerInstrumentationDiscoveryService Service
        {
            get { return _lazyDiscoveryService.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryService"></param>
        public InstallerContext(IInstallerInstrumentationDiscoveryService discoveryService)
        {
            _lazyDiscoveryService = new Lazy<IInstallerInstrumentationDiscoveryService>(() =>
            {
                discoveryService.Discover();
                return discoveryService;
            });
        }

        public virtual void Install()
        {
            Uninstall();

            foreach (var category in Service.CategoryDescriptors)
            {
                var pcc = category.CreateCategory();

                if (pcc != null) continue;

                var message = string.Format("Unable to install the {0} '{1}' definition.",
                    typeof(PerformanceCounterCategory), category.Name);

                throw new InvalidOperationException(message);
            }
        }

        public Task InstallAsync()
        {
            return Task.Run(() => Install());
        }

        public virtual void Uninstall()
        {
            foreach (var category in Service.CategoryDescriptors)
            {
                if (category.TryDeleteCategory()) continue;

                var message = string.Format("Unable to uninstall the {0} '{1}' definition.",
                    typeof(PerformanceCounterCategory), category.Name);

                throw new InvalidOperationException(message);
            }
        }

        public Task UninstallAsync()
        {
            return Task.Run(() => Uninstall());
        }
    }
}