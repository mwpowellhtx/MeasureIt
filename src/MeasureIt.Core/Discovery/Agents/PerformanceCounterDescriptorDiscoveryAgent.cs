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

        private static void AddOrReplace(IList<IPerformanceCounterDescriptor> descriptors,
            IPerformanceCounterDescriptor descriptor)
        {
            // Unable to consider a Descriptor under these conditions.
            if (descriptor == null
                || descriptor.Method == null
                || descriptor.Method.DeclaringType == null)
            {
                const string message = "Unable to consider descriptor";
                throw new ArgumentException(message, "descriptor");
            }

            var existing = descriptors.FirstOrDefault(
                d => d.CategoryType == descriptor.CategoryType
                     && d.AdapterType == descriptor.AdapterType
                     && d.Method.GetBaseDefinition() == descriptor.Method.GetBaseDefinition());

            if (existing == null)
            {
                descriptors.Add(descriptor);
                return;
            }

            if (existing.Method == null || existing.Method.DeclaringType == null)
            {
                const string message = "Unable to consider descriptors";
                throw new ArgumentException(message, "descriptors");
            }

            // No relation to the existing Method one way or another.
            if (!(existing.Method.DeclaringType.IsAssignableFrom(descriptor.Method.DeclaringType)
                  || descriptor.Method.DeclaringType.IsAssignableFrom(existing.Method.DeclaringType)))
            {
            }

            if (!descriptor.Method.DeclaringType.IsSubclassOf(existing.Method.DeclaringType))
            {
                return;
            }

            var i = descriptors.IndexOf(existing);

            // Replace the existing Descriptor with one that would supersede it.
            descriptors[i] = descriptor;
        }

        protected override IEnumerable<IPerformanceCounterDescriptor> DiscoverValues(
            InstrumentationDiscovererOptions options, IEnumerable<Type> exportedTypes)
        {
            var o = options;

            // ReSharper disable once PossibleMultipleEnumeration
            var concreteClassTypes = exportedTypes.Where(
                type => type.IsClass && !type.IsAbstract).ToArray();

            var decoratedMethods = concreteClassTypes.SelectMany(
                type => type.GetMethods(o.MethodBindingAttr).Where(
                    method => method.HasAttributes<PerformanceCounterAttribute>(o.IncludeInherited))).ToArray();

            // TODO: TBD: unique "key" is: CategoryType, AdapterType
            var potentialDescriptors = decoratedMethods.SelectMany(
                method => method.GetCustomAttributes<PerformanceCounterAttribute>(o.IncludeInherited)
                    .Select(a =>
                    {
                        a.Descriptor.Method = method;
                        return a.Descriptor;
                    }))
                .ToArray();

            Func<IPerformanceCounterDescriptor, bool> hasMethod = d
                => !(d.Method == null || d.Method.ReflectedType == null
                     || d.Method.DeclaringType == null);

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            var descriptors = potentialDescriptors.Filter((items, x)
                => hasMethod(x) && items.Any(i
                    => hasMethod(i)
                       && x.Method.ReflectedType == i.Method.ReflectedType
                       && x.Method.GetBaseDefinition() == i.Method.GetBaseDefinition()
                       && x.Method.DeclaringType != i.Method.DeclaringType
                       && x.Method.DeclaringType.IsSubclassOf(i.Method.DeclaringType)
                    )
                ).ToArray();

            return descriptors;
        }
    }
}
