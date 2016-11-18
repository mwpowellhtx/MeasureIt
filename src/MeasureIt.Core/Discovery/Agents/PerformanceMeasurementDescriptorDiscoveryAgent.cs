using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptorDiscoveryAgent : DiscoveryAgentBase<
        IPerformanceMeasurementDescriptor>, IPerformanceMeasurementDescriptorDiscoveryAgent
    {
        internal PerformanceMeasurementDescriptorDiscoveryAgent(
            IInstrumentationDiscoveryOptions options
            , DiscoveryServiceExportedTypesGetterDelegate getExportedTypes
            )
            : base(options, getExportedTypes)
        {
            _filter = new PerformanceMeasurementDescriptorFilter();
        }

        private readonly PerformanceMeasurementDescriptorFilter _filter;

        private class PerformanceMeasurementDescriptorFilter : EqualityComparer<
            IPerformanceMeasurementDescriptor>
        {
            private readonly MethodInfoEqualityComparer _comparer;

            internal PerformanceMeasurementDescriptorFilter()
            {
                _comparer = new MethodInfoEqualityComparer();
            }

            public override bool Equals(IPerformanceMeasurementDescriptor x,
                IPerformanceMeasurementDescriptor y)
            {
                // Equate X and Y based on their Method Base Definitions.
                var xMethodBaseDef = x.Method.GetBaseDefinition();
                var yMethodBaseDef = y.Method.GetBaseDefinition();
                return _comparer.Equals(xMethodBaseDef, yMethodBaseDef);
            }

            public override int GetHashCode(IPerformanceMeasurementDescriptor a)
            {
                return a == null ? 0 : _comparer.GetHashCode(a.Method);
            }
        }

        private IEnumerable<IPerformanceMeasurementDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions options, Type rootType, Type currentType)
        {
            const bool inherited = false;

            /* TODO: TBD: this does not yet take into consideration static methods.
             * that could potentially deserve a whole other strain of Discovery Agents. */

            var o = options;

            var bases = (currentType.BaseType != null
                ? DiscoverValues(o, rootType, currentType.BaseType)
                : new List<IPerformanceMeasurementDescriptor>()).ToArray();

            /* TODO: TBD: RootType is necessary because, according to this strategy, Method will
             * be focused on the CurrentType as the ReflectedType, which may or may not work when
             * it comes time to formulate interception contexts. Currently at least operating
             * under the theory that we will need the rooted Type information. */

            var currents = currentType.GetMethods(o.MethodBindingAttr)
                .Where(m => m.HasAttributes<MeasurePerformanceAttribute>(inherited))
                .SelectMany(method => method.GetAttributeValues(
                    (MeasurePerformanceAttribute a) => a.Descriptor).Select(d =>
                    {
                        var cloned = (IPerformanceMeasurementDescriptor) d.Clone();
                        cloned.RootType = rootType;
                        cloned.Method = method.GetBaseDefinition();
                        return cloned;
                    })).ToArray();

#if DEBUG
            var currentPrefixes = bases.Select(x => x.Prefix).ToArray();
            var potentialPrefixes = currents.Select(x => x.Prefix).ToArray();
#endif
            /* Except with _filter does not work properly, or we have not provided a good enough
             * hash code. This should work, however, and leverages the Equals method. */

            var forwarded = bases.Where(b => !currents.Any(c => _filter.Equals(b, c))).ToArray();
            //var forwarded = currents.Where(c => potentials.All(p => !p.Equals(c))).ToArray();

#if DEBUG
            const string prefix = "MeasureIt.SubjectClassWithNonPublicMethods.MethodDeclaredInBaseOnly";
            var any = currents.Any(x => x.Prefix == prefix) && forwarded.Any(x => x.Prefix == prefix);
            var count = currents.Count(x => x.Prefix == prefix) + forwarded.Count(x => x.Prefix == prefix);
            var methods = currents.Where(x => x.Prefix == prefix).Select(x => x.Method)
                .Concat(forwarded.Where(x => x.Prefix == prefix).Select(x => x.Method)).ToArray();
            var baseItems = bases.Where(x => x.Prefix == prefix).ToArray();
            var currentItems = currents.Where(x => x.Prefix == prefix).ToArray();
            var forwardedItems = forwarded.Where(x => x.Prefix == prefix).ToArray();
            var allItems = forwarded.Concat(currents).Where(x => x.Prefix == prefix).ToArray();
            var forwardedPrefixes = forwarded.Select(x => x.Prefix).ToArray();
#endif

            /* Keep all of our Ps, Ds, and Qs in proper working order, meaning that we pull
             * forward Base Descriptors that are not represented by the Potential Descriptors. */

            // Then we may return the Potential Descriptors.

            foreach (var d in forwarded.Concat(currents))
                yield return d;
        }

        ///// <summary>
        ///// The most recently appearing, overshadowed <see cref="MeasurePerformanceAttribute"/>,
        ///// may not necessarily be the most recently declared, overriden <see cref="MethodInfo"/>.
        ///// So we must go about aligning the <see cref="PerformanceMeasurementDescriptor"/> with
        ///// the correct one.
        ///// </summary>
        ///// <param name="options"></param>
        ///// <param name="descriptor"></param>
        ///// <param name="currentType"></param>
        //private static void AlignDescriptorMethod(IInstrumentationDiscoveryOptions options
        //    , IMeasurementOptions descriptor, Type currentType = null)
        //{
        //    var o = options;

        //    currentType = currentType ?? descriptor.RootType;

        //    while (currentType != null)
        //    {
        //        var currentMethod = descriptor.Method;

        //        var overriddenMethod = currentType.GetMethods(o.MethodBindingAttr)
        //            .FirstOrDefault(
        //                m => m.GetBaseDefinition() == currentMethod.GetBaseDefinition());

        //        if (overriddenMethod != null)
        //        {
        //            descriptor.Method = overriddenMethod;
        //            return;
        //        }

        //        currentType = currentType.BaseType;
        //    }
        //}

        protected override IEnumerable<IPerformanceMeasurementDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var d in exportedTypes.Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => DiscoverValues(o, t, t)))
            {
                //// "Oh Dear", Align Descriptor Method with the most recent Overridden Method.
                //AlignDescriptorMethod(o, d);

                yield return d;
            }
        }
    }
}
