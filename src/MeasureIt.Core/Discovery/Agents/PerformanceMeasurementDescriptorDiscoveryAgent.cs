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

            var o = options;

            var bases = (currentType.BaseType != null
                ? DiscoverValues(o, rootType, currentType.BaseType)
                : new List<IPerformanceMeasurementDescriptor>()).ToArray();

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

            /* Except with _filter does not work properly, or we have not provided a good enough
             * hash code. This should work, however, and leverages the Equals method. */

            var forwarded = bases.Where(b => !currents.Any(c => _filter.Equals(b, c))).ToArray();

            /* Keep all of our Ps, Ds, and Qs in proper working order, meaning that we pull
             * forward Base Descriptors that are not represented by the Potential Descriptors. */

            // Then we may return the Potential Descriptors.
            foreach (var d in forwarded.Concat(currents))
                yield return d;
        }

        protected override IEnumerable<IPerformanceMeasurementDescriptor> DiscoverValues(
            IInstrumentationDiscoveryOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            // There is nothing we use from the base class except to vet the parameters themselves.

            // ReSharper disable once IteratorMethodResultIsIgnored, PossibleMultipleEnumeration
            base.DiscoverValues(options, exportedTypes);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var d in exportedTypes.Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => DiscoverValues(o, t, t)))
            {
                yield return d;
            }
        }
    }
}
