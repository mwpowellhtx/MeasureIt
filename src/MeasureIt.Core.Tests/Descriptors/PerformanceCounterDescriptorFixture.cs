using System;
using System.Linq;

namespace MeasureIt.Descriptors
{
    public class PerformanceCounterDescriptorFixture : PerformanceCounterDescriptor
        , IPerformanceCounterDescriptorFixture
        , IEquatable<PerformanceCounterDescriptorFixture>
        , ISimilarity<PerformanceCounterDescriptorFixture>
    {
        internal PerformanceCounterDescriptorFixture(Type adapterType, Type categoryType)
            : base(adapterType, categoryType)
        {
        }

        internal PerformanceCounterDescriptorFixture(IPerformanceCounterDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPerformanceCounterDescriptorFixture other)
        {
            return Equals(this, other);
        }

        public bool Equals(PerformanceCounterDescriptorFixture other)
        {
            return Equals(this, other);
        }

        private static bool IsSimilarTo(IPerformanceCounterDescriptor a, IPerformanceCounterDescriptor b)
        {
            return ReferenceEquals(a, b)
                   || (
                       !(a.Method == null || b.Method == null)
                       && a.Method.GetBaseDefinition() == b.Method.GetBaseDefinition()
                       && !(a.RootType == null || b.RootType == null)
                       && (a.RootType.IsSubclassOf(b.RootType)
                           || b.RootType.IsSubclassOf(a.RootType))
                       && !(a.CategoryType == null || b.CategoryType == null
                            || a.AdapterTypes == null || b.AdapterTypes == null)
                       && a.CategoryType == b.CategoryType
                       && a.AdapterTypes.SequenceEqual(b.AdapterTypes)
                       );
        }

        public bool IsSimilarTo(IPerformanceCounterDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }

        public bool IsSimilarTo(PerformanceCounterDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }
    }
}
