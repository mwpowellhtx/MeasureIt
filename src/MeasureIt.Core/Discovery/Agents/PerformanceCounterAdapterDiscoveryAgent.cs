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
        private IEnumerable<IPerformanceCounterAdapter> _adapters;

        public virtual IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _adapters; }
            private set { _adapters = (value ?? new List<IPerformanceCounterAdapter>()).ToArray(); }
        }

        internal PerformanceCounterAdapterDiscoveryAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
            : base(options, getExportedTypes)
        {
        }

        protected override IEnumerable<IPerformanceCounterAdapter> DiscoverValues(
            IInstrumentationDiscoveryOptions options, IEnumerable<Type> exportedTypes)
        {
            var adapterType = typeof(IPerformanceCounterAdapter);

            // TODO: TBD: re-fit this one to include include inherited discernment
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
