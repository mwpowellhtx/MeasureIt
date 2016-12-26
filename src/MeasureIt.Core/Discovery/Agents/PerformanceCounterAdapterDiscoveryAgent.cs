using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterAdapterDiscoveryAgent : DiscoveryAgentBase<
        IPerformanceCounterAdapter>, IPerformanceCounterAdapterDiscoveryAgent
    {
        internal PerformanceCounterAdapterDiscoveryAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
            : base(options, getExportedTypes)
        {
        }

        /// <summary>
        /// Discovers the values given <paramref name="options"/> and <paramref name="exportedTypes"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="exportedTypes"></param>
        /// <returns></returns>
        protected override IEnumerable<IPerformanceCounterAdapter> DiscoverValues(
            IInstrumentationDiscoveryOptions options, IEnumerable<Type> exportedTypes)
        {
            var adapterType = typeof(IPerformanceCounterAdapter);

            // There is nothing we use from the base class except to vet the parameters themselves.

            // ReSharper disable once IteratorMethodResultIsIgnored, PossibleMultipleEnumeration
            base.DiscoverValues(options, exportedTypes);

            // TODO: TBD: re-fit this one to include include inherited discernment

            // ReSharper disable once PossibleMultipleEnumeration
            var types = exportedTypes.Where(
                type => type.IsClass && !type.IsAbstract
                        && adapterType.IsAssignableFrom(type)
                );

            var bindignAttr = options.ConstructorBindingAttr;

            foreach (var adapter in types.Select(type => type.CreateInstance<IPerformanceCounterAdapter>(bindignAttr)))
                yield return adapter;
        }
    }
}
