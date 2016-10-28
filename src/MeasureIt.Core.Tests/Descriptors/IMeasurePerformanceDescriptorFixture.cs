using System;

namespace MeasureIt.Descriptors
{
    public interface IMeasurePerformanceDescriptorFixture
        : IMeasurePerformanceDescriptor
            , IEquatable<IMeasurePerformanceDescriptorFixture>
            , ISimilarity<IMeasurePerformanceDescriptorFixture>
    {
    }
}
