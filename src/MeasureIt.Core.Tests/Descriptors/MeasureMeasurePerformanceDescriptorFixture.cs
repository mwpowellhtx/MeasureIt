using System;
using System.Linq;

namespace MeasureIt.Descriptors
{
    public class MeasureMeasurePerformanceDescriptorFixture : MeasurePerformanceDescriptor
        , IMeasurePerformanceDescriptorFixture
        , IEquatable<MeasureMeasurePerformanceDescriptorFixture>
        , ISimilarity<MeasureMeasurePerformanceDescriptorFixture>
    {
        internal MeasureMeasurePerformanceDescriptorFixture(Type adapterType, Type categoryType)
            : base(adapterType, categoryType)
        {
        }

        internal MeasureMeasurePerformanceDescriptorFixture(IMeasurePerformanceDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IMeasurePerformanceDescriptorFixture other)
        {
            return Equals(this, other);
        }

        public bool Equals(MeasureMeasurePerformanceDescriptorFixture other)
        {
            return Equals(this, other);
        }

        private static bool IsSimilarTo(IMeasurePerformanceDescriptor a, IMeasurePerformanceDescriptor b)
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

        public bool IsSimilarTo(IMeasurePerformanceDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }

        public bool IsSimilarTo(MeasureMeasurePerformanceDescriptorFixture other)
        {
            return IsSimilarTo(this, other);
        }
    }
}
