using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = ErrorRateType, Help = "Number of errors per second.")]
    public class ErrorRatePerformanceCounterAdapter : PerformanceCounterAdapterBase<
        ErrorRatePerformanceCounterAdapter>
    {
        internal ErrorRatePerformanceCounterAdapter()
        {
        }

        internal const PerformanceCounterType ErrorRateType = PerformanceCounterType.RateOfCountsPerSecond64;

        private PerformanceCounter ErrorRateCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == ErrorRateType); }
        }

        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do here.
        }

        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            // TODO: TBD: error rate, or errors per second, implies we also need the time component?
            if (!descriptor.HasError) return;
            ErrorRateCounter.Increment();
        }
    }
}
