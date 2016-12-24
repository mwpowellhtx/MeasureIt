using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<IInstallerInstrumentationDiscoveryService>(() =>
            {
                // Discover if we have not already done so.
                discoveryService.Discover();
                return discoveryService;
            }, execAndPubThreadSafety);
        }

        /// <summary>
        /// Installs the set of Performance Counter Categories.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when an error occurs installing
        /// a Performance Counter Category.</exception>
        public virtual void Install()
        {
            // TODO: TBD: may not need this context quite as much as "simply" the extension methods...
            Uninstall();

            var throwOnInstallerFailure = Options.ThrowOnInstallerFailure;

            using (var adapter = new PerformanceCounterCategoryInstallerContextAdapter(Service.CategoryAdapters.Values))
            {
                foreach (var tuple in adapter.GetInstalledCategories())
                {
                    if (tuple.Item2 != null || throwOnInstallerFailure) continue;

                    // Leverages C# 6.0 language features.
                    var message = $"Unable to install the {typeof(PerformanceCounterCategory)} '{tuple.Item1.Name}' definition.";

                    throw new InvalidOperationException(message);
                }
            }
        }

        /// <summary>
        /// <see cref="Install"/> the categories asynchronously.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when an error occurs uninstalling
        /// a Performance Counter Category.</exception>
        public Task InstallAsync()
        {
            return Task.Run(() => Install());
        }

        /// <summary>
        /// Uninstalls the Performance Counter Categories.
        /// </summary>
        public virtual void Uninstall()
        {
            var throwOnInstallerFailure = Options.ThrowOnInstallerFailure;

            // TODO: TBD: ditto Install re: extension methods...
            using (var adapter = new PerformanceCounterCategoryUninstallerContextAdapter(Service.CategoryAdapters.Values))
            {
                IEnumerable<string> categoryNames;

                var results = adapter.TryUninstallCategories(out categoryNames);

                var unable = results.Zip(categoryNames,
                    (result, name) => new {Result = result.Item2, Name = name})
                    .Where(z => !z.Result).ToArray();

                if (!unable.Any() || !throwOnInstallerFailure) return;

                // Ditto C# 6.0 language features.
                var message = $"Unable to uninstall the {typeof(PerformanceCounterCategory)} '{string.Join(", ", unable.Select(x => x.Name))}' definitions.";

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// <see cref="Uninstall"/> the categories asynchronously.
        /// </summary>
        /// <returns></returns>
        public Task UninstallAsync()
        {
            return Task.Run(() => Uninstall());
        }
    }
}