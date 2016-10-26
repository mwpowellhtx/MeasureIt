using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterCategoryDescriptorDiscoveryAgent : DiscoveryAgentBase<
        IPerformanceCounterCategoryDescriptor>, IPerformanceCounterCategoryDescriptorDiscoveryAgent
    {
        internal PerformanceCounterCategoryDescriptorDiscoveryAgent(
            IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
            : base(options, getExportedTypes)
        {
        }

        protected override IEnumerable<IPerformanceCounterCategoryDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            var categoryAdapterType = typeof(IPerformanceCounterCategoryAdapter);

            // Literally, type is categoryAdapterType.
            var decoratedCategoryTypes = exportedTypes.Where(
                type => type.IsClass && !type.IsAbstract
                        && categoryAdapterType.IsAssignableFrom(type)
                        && type.HasAttribute<PerformanceCounterCategoryAttribute>(o.IncludeInherited)
                ).ToArray();

            // Assigning the Category.Type is ULTRA critical in order to align with Counter Creation Data.
            var descriptors = decoratedCategoryTypes.Select(type => type.GetAttributeValue(
                (PerformanceCounterCategoryAttribute a) =>
                {
                    var d = a.Descriptor;
                    d.Type = type;
                    return d;
                })).ToArray();

            // TODO: TBD: will want a Filter on this one

            return descriptors;
        }
    }
}
