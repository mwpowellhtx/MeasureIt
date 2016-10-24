using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterAdapterDescriptorDiscoveryAgent : DiscoveryAgentBase<
        IPerformanceCounterAdapterDescriptor>, IPerformanceCounterAdapterDescriptorDiscoveryAgent
    {
        internal PerformanceCounterAdapterDescriptorDiscoveryAgent(
            InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
            : base(options, getExportedTypes)
        {
        }

        protected override IEnumerable<IPerformanceCounterAdapterDescriptor> DiscoverValues(
            InstrumentationDiscovererOptions options, IEnumerable<Type> exportedTypes)
        {
            var adapterType = typeof(IPerformanceCounterAdapter);

            // TODO: TBD: re-fit this one to include include inherited discernment
            var types = exportedTypes.Where(
                type => type.IsClass && !type.IsAbstract
                        && adapterType.IsAssignableFrom(type)
                        && type.HasAttribute<PerformanceCounterAdapterAttribute>()
                );

            // The Agent must set the CounterAdapterType.

            var descriptors = types.Select(type => type.GetAttributeValue(
                (PerformanceCounterAdapterAttribute a) =>
                {
                    var d = a.Descriptor;
                    d.AdapterType = type;
                    return d;
                })).ToArray();

            return descriptors;
        }
    }
}
