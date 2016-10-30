using System;

namespace MeasureIt.Descriptors
{
    public interface IPerformanceMeasurementDescriptorFixture
        : IPerformanceMeasurementDescriptor
            , IEquatable<IPerformanceMeasurementDescriptorFixture>
            , ISimilarity<IPerformanceMeasurementDescriptorFixture>
    {
    }
}
