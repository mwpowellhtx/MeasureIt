using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TDescriptor"></typeparam>
    /// <typeparam name="TAttribute"></typeparam>
    public class PerformanceMeasurementDescriptorDiscoveryAgentBase<TDescriptor, TAttribute>
        : DiscoveryAgentBase<IPerformanceMeasurementDescriptor>
            , IPerformanceMeasurementDescriptorDiscoveryAgent<TAttribute>
        where TDescriptor : class, IPerformanceMeasurementDescriptor
        where TAttribute : Attribute, IMeasurePerformanceAttribute<TDescriptor>
    {
        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <param name="getExportedTypes"></param>
        protected PerformanceMeasurementDescriptorDiscoveryAgentBase(
            IInstrumentationDiscoveryOptions discoveryOptions
            , DiscoveryServiceExportedTypesGetterDelegate getExportedTypes
        )
            : base(discoveryOptions, getExportedTypes)
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
                // This is when we want the Base Definition.
                return _comparer.Equals(xMethodBaseDef, yMethodBaseDef);
            }

            public override int GetHashCode(IPerformanceMeasurementDescriptor a)
            {
                return a == null ? 0 : _comparer.GetHashCode(a.Method);
            }
        }

        /* TODO: TBD: but for the Attribute type possibly being injected via generics, this could
         * potentially be fed from a base class for both "straight" injection, as well as for WebApi
         * (and later, Mvc). */

        private IEnumerable<IPerformanceMeasurementDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions discoveryOptions, Type rootType, Type currentType)
        {
            const bool inherited = false;

            var o = discoveryOptions;

            var bases = (currentType.BaseType != null
                ? DiscoverValues(o, rootType, currentType.BaseType)
                : new List<IPerformanceMeasurementDescriptor>()).ToArray();

            var currents = currentType.GetMethods(o.MethodBindingAttr)
                .Where(m => m.HasAttributes<TAttribute>(inherited))
                .SelectMany(method => method.GetAttributeValues(
                    (TAttribute a) => a.Descriptor).Select(d =>
                    {
                        var cloned = (IPerformanceMeasurementDescriptor) d.Clone();
                        cloned.RootType = rootType;
                        cloned.Method = method.GetBaseDefinition();
                        return cloned;
                    })).ToArray();

            /* Except with _filter does not work properly, or we have not provided a good enough
             * hash code. This should work, however, and leverages the Equals method. */

            var forwarded = bases.Where(b => !currents.Any(c => _filter.Equals(b, c))).ToArray();

            /* Keep all of our Ps, Ds, and Qs in proper working order, meaning that we pull
             * forward Base Descriptors that are not represented by the Potential Descriptors. */

            // Then we may return the Potential Descriptors.
            foreach (var d in forwarded.Concat(currents))
                yield return d;
        }

        /// <summary>
        /// Discovers the descriptors providing <paramref name="discoveryOptions"/> and
        /// <paramref name="exportedTypes"/>.
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <param name="exportedTypes"></param>
        /// <returns></returns>
        protected override IEnumerable<IPerformanceMeasurementDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions discoveryOptions, IEnumerable<Type> exportedTypes)
        {
            var o = discoveryOptions;

            // There is nothing we use from the base class except to vet the parameters themselves.

            // ReSharper disable once IteratorMethodResultIsIgnored, PossibleMultipleEnumeration
            base.DiscoverValues(o, exportedTypes);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var d in exportedTypes.Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => DiscoverValues(o, t, t)))
            {
                yield return d;
            }
        }
    }
}
