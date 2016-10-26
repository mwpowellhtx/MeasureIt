using System;

namespace MeasureIt.Descriptors
{
    public interface IPerformanceCounterDescriptorFixture
        : IPerformanceCounterDescriptor
            , IEquatable<IPerformanceCounterDescriptorFixture>
            , ISimilarity<IPerformanceCounterDescriptorFixture>
    {
    }
}
