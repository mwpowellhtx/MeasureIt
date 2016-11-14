using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public class InstallerContext : ContextBase, IInstallerContext
    {
        private IInstrumentationDiscoveryOptions Options { get; set; }

        private readonly Lazy<IInstallerInstrumentationDiscoveryService> _lazyDiscoveryService;

        private IInstallerInstrumentationDiscoveryService Service
        {
            get { return _lazyDiscoveryService.Value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="discoveryService"></param>
        public InstallerContext(IInstrumentationDiscoveryOptions options
            , IInstallerInstrumentationDiscoveryService discoveryService)
        {
            Options = options;

            _lazyDiscoveryService = new Lazy<IInstallerInstrumentationDiscoveryService>(() =>
            {
                // Discover if we have not already done so.
                discoveryService.Discover();
                return discoveryService;
            });
        }

        public virtual void Install()
        {
            Uninstall();

            var throwOnInstallerFailure = Options.ThrowOnInstallerFailure;

            foreach (var category in Service.CategoryAdapters.Where(a => a.CreationData.Any()))
            {
                var pcc = category.CreateCategory();

                if (pcc != null || !throwOnInstallerFailure) continue;

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
            var throwOnInstallerFailure = Options.ThrowOnInstallerFailure;

            foreach (var category in Service.CategoryAdapters)
            {
                if (category.TryDeleteCategory() || !throwOnInstallerFailure) continue;

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