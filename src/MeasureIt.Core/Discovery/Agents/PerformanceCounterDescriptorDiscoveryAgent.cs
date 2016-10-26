using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            InstrumentationDiscovererOptions options, Type rootType, Type currentType)
        {
            const bool inherited = false;

            /* TODO: TBD: this does not yet take into consideration static methods.
             * that could potentially deserve a whole other strain of Discovery Agents. */

            var o = options;

            var bases = (currentType.BaseType != null
                ? DiscoverValues(o, rootType, currentType.BaseType)
                : new List<IPerformanceCounterDescriptor>()).ToArray();

            /* TODO: TBD: RootType is necessary because, according to this strategy, Method will
             * be focused on the CurrentType as the ReflectedType, which may or may not work when
             * it comes time to formulate interception contexts. Currently at least operating
             * under the theory that we will need the rooted Type information. */

            var potentials = currentType.GetMethods(o.MethodBindingAttr)
                .Where(m => m.HasAttributes<PerformanceCounterAttribute>(inherited))
                .SelectMany(method => method.GetAttributeValues(
                    (PerformanceCounterAttribute a) => a.Descriptor).Select(d =>
                    {
                        d.RootType = rootType;
                        d.Method = method;
                        return d;
                    })).ToArray();

            var forwarded = bases.Where(b => potentials.All(p => !p.Equals(b))).ToArray();

            /* Keep all of our Ps, Ds, and Qs in proper working order, meaning that we pull
             * forward Base Descriptors that are not represented by the Potential Descriptors. */

            // Then we may return the Potential Descriptors.

            foreach (var d in forwarded.Concat(potentials))
                yield return d;
        }

        /// <summary>
        /// The most recently appearing, overshadowed <see cref="PerformanceCounterAttribute"/>,
        /// may not necessarily be the most recently declared, overriden <see cref="MethodInfo"/>.
        /// So we must go about aligning the <see cref="PerformanceCounterDescriptor"/> with the
        /// correct one.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="descriptor"></param>
        /// <param name="currentType"></param>
        private static void AlignDescriptorMethod(InstrumentationDiscovererOptions options
            , IMeasurementOptions descriptor, Type currentType = null)
        {
            var o = options;

            currentType = currentType ?? descriptor.RootType;

            while (currentType != null)
            {
                var currentMethod = descriptor.Method;

                var overriddenMethod = currentType.GetMethods(o.MethodBindingAttr)
                    .FirstOrDefault(
                        m => m.GetBaseDefinition() == currentMethod.GetBaseDefinition());

                if (overriddenMethod != null)
                {
                    descriptor.Method = overriddenMethod;
                    return;
                }

                currentType = currentType.BaseType;
            }
        }

        protected override IEnumerable<IPerformanceCounterDescriptor> DiscoverValues(
            InstrumentationDiscovererOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var d in exportedTypes.Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => DiscoverValues(o, t, t)))
            {
                // "Oh Dear", Align Descriptor Method with the most recent Overridden Method.
                AlignDescriptorMethod(o, d);

                yield return d;
            }
        }
    }
}
