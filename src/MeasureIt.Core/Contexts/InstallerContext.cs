using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    using Adapters;
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

            using (var adapter = new PerformanceCounterCategoryInstallerContextAdapter(Service.CategoryAdapters))
            {
                foreach (var tuple in adapter.GetInstalledCategories())
                {
                    if (tuple.Item2 != null || throwOnInstallerFailure) continue;

                    var message = string.Format("Unable to install the {0} '{1}' definition.",
                        typeof(PerformanceCounterCategory), tuple.Item1.Name);

                    throw new InvalidOperationException(message);
                }
            }
        }

        public Task InstallAsync()
        {
            return Task.Run(() => Install());
        }

        public virtual void Uninstall()
        {
            var throwOnInstallerFailure = Options.ThrowOnInstallerFailure;

            using (var adapter = new PerformanceCounterCategoryUninstallerContextAdapter(Service.CategoryAdapters))
            {

                IEnumerable<string> categoryNames;

                var results = adapter.TryUninstallCategories(out categoryNames);

                var unable = results.Zip(categoryNames,
                    (result, name) => new {Result = result.Item2, Name = name})
                    .Where(z => !z.Result).ToArray();

                if (!unable.Any() || !throwOnInstallerFailure) return;

                var message = string.Format("Unable to uninstall the {0} '{1}' definitions.",
                    typeof(PerformanceCounterCategory), string.Join(", ", unable.Select(x => x.Name)));

                throw new InvalidOperationException(message);
            }
        }

        public Task UninstallAsync()
        {
            return Task.Run(() => Uninstall());
        }
    }
}