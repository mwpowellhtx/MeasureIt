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
            : base("error rate", "Number of errors per second (Hz).")
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
            ErrorRateCounter.Increment();
        }
    }
}
