using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt.Discovery
{
    using Adapters;
    using Contexts;

    /// <summary>
    /// 
    /// </summary>
    public interface IInstallerInstrumentationDiscoveryService : IRuntimeInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IInstallerContext GetInstallerContext();
    }

    /// <summary>
    /// 
    /// </summary>
    public static class InstallerInstrumentationDiscoveryServiceExtensionMethods
    {
        /// <summary>
        /// Performs an Install given the <paramref name="service"/>. Discovers the Categories
        /// as an immediate precursor to the Install request.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        /// <param name="discoveryOptions"></param>
        /// <param name="respond"></param>
        /// <returns></returns>
        public static TService Install<TService>(this TService service
            , IInstrumentationDiscoveryOptions discoveryOptions
            , Action<IEnumerable<Tuple<IPerformanceCounterCategoryAdapter
                , PerformanceCounterCategory>>> respond = null)
            where TService : IInstallerInstrumentationDiscoveryService
        {
            service.Discover();

            respond = respond ?? delegate { };

            using (var adapter = new PerformanceCounterCategoryInstallerContextAdapter(
                discoveryOptions, service.CategoryAdapters.Values))
            {
                // We need to make sure that the Categories are resolved through and through.
                var categories = adapter.GetInstalledCategories().ToArray();
                respond(categories);
            }

            return service;
        }

        /// <summary>
        /// Performs an Uninstall given the <paramref name="service"/>.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="service"></param>
        /// <param name="discoveryOptions"></param>
        /// <returns></returns>
        public static bool TryUninstall<TService>(this TService service, IInstrumentationDiscoveryOptions discoveryOptions)
            where TService : IInstallerInstrumentationDiscoveryService
        {
            using (var adapter = new PerformanceCounterCategoryUninstallerContextAdapter(
                discoveryOptions, service.CategoryAdapters.Values))
            {
                IEnumerable<string> categoryNames;
                return adapter.TryUninstallCategories(out categoryNames).All(tuple => tuple.Item2);
            }
        }
    }
}
