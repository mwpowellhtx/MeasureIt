using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterDescriptorDiscoveryAgent : DiscoveryAgentBase<
        IPerformanceCounterDescriptor>, IPerformanceCounterDescriptorDiscoveryAgent
    {
        internal PerformanceCounterDescriptorDiscoveryAgent(InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
            : base(options, getExportedTypes)
        {
        }

        private static IEnumerable<IPerformanceCounterDescriptor> DiscoverValues(
            InstrumentationDiscovererOptions options, Type type)
        {
            const bool inherited = false;

            var o = options;

            var baseDescriptors = (type.BaseType != null
                ? DiscoverValues(o, type.BaseType)
                : new List<IPerformanceCounterDescriptor>()).ToArray();

            var potentialDescriptors = type.GetMethods(o.MethodBindingAttr)
                .Where(m => m.HasAttributes<PerformanceCounterAttribute>(inherited))
                .SelectMany(method => method.GetAttributeValues(
                    (PerformanceCounterAttribute a) => a.Descriptor).Select(d =>
                    {
                        d.Method = method;
                        return d;
                    })).ToArray();

            // Keep all of our Ps, Ds, and Qs in proper working order.
            foreach (var d in baseDescriptors)
            {
                if (potentialDescriptors.Any(p => p.Equals(d))) continue;
                yield return d;
            }

            // Then we may return the Potential Descriptors.
            foreach (var p in potentialDescriptors)
                yield return p;
        }

        protected override IEnumerable<IPerformanceCounterDescriptor> DiscoverValues(
            InstrumentationDiscovererOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var d in exportedTypes.Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => DiscoverValues(o, t)))
            {
                yield return d;
            }
        }
    }
}
