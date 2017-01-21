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
    using static LazyThreadSafetyMode;

    /// <summary>
    /// 
    /// </summary>
    public class InstallerContext : ContextBase, IInstallerContext
    {
        private IInstrumentationDiscoveryOptions DiscoveryOptions { get; }

        private readonly Lazy<IInstallerInstrumentationDiscoveryService> _lazyDiscoveryService;

        private IInstallerInstrumentationDiscoveryService Service => _lazyDiscoveryService.Value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <param name="discoveryService"></param>
        public InstallerContext(IInstrumentationDiscoveryOptions discoveryOptions
            , IInstallerInstrumentationDiscoveryService discoveryService)
        {
            DiscoveryOptions = discoveryOptions;

            _lazyDiscoveryService = new Lazy<IInstallerInstrumentationDiscoveryService>(() =>
            {
                // Discover if we have not already done so.
                discoveryService.Discover();
                return discoveryService;
            }, ExecutionAndPublication);
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

            var o = DiscoveryOptions;

            var throwOnInstallerFailure = o.ThrowOnInstallerFailure;

            using (var adapter = new PerformanceCounterCategoryInstallerContextAdapter(
                Service.CategoryAdapters.Values))
            {
                foreach (var tuple in adapter.GetInstalledCategories())
                {
                    if (tuple.Item2 != null || !throwOnInstallerFailure) continue;

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

        private class UninstallState
        {
            /// <summary>
            /// Gets or sets the Result of the <see cref="Uninstall"/> operation.
            /// </summary>
            internal bool Result { get; set; }

            /// <summary>
            /// Gets or sets the Name of the <see cref="Uninstall"/> state.
            /// </summary>
            internal string Name { get; set; }
        }

        /// <summary>
        /// Uninstalls the Performance Counter Categories.
        /// </summary>
        public virtual void Uninstall()
        {
            var o = DiscoveryOptions;

            var throwOnUninstallerFailure = o.ThrowOnUninstallerFailure;

            // TODO: TBD: ditto Install re: extension methods...
            using (var adapter = new PerformanceCounterCategoryUninstallerContextAdapter(
                Service.CategoryAdapters.Values))
            {
                IEnumerable<string> categoryNames;

                var results = adapter.TryUninstallCategories(out categoryNames);

                var unable = results.Zip(categoryNames,
                        (result, name) => new UninstallState {Result = result.Item2, Name = name})
                    .Where(z => !z.Result).ToArray();

                if (!unable.Any() || !throwOnUninstallerFailure) return;

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