using System;
using System.Linq;

namespace MeasureIt.Descriptors
{
    public class PerformanceMeasurementDescriptorFixture : PerformanceMeasurementDescriptor
        , IPerformanceMeasurementDescriptorFixture
        , IEquatable<PerformanceMeasurementDescriptorFixture>
        , ISimilarity<PerformanceMeasurementDescriptorFixture>
    {
        internal PerformanceMeasurementDescriptorFixture(Type adapterType, Type categoryType)
            : base(adapterType, categoryType)
        {
        }

        internal PerformanceMeasurementDescriptorFixture(IPerformanceMeasurementDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPerformanceMeasurementDescriptorFixture other)
        {
            return Equals(this, other);
        }

        public bool Equals(PerformanceMeasurementDescriptorFixture other)
        {
            return Equals(this, other);
        }

        private static bool IsSimilarTo(IPerformanceMeasurementDescriptor a, IPerformanceMeasurementDescriptor b)
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

        public bool IsSimilarTo(IPerformanceMeasurementDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }

        public bool IsSimilarTo(PerformanceMeasurementDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }
    }
}
