using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [PerformanceCounterAdapter("error rate", Help = "Number of errors per second (Hz).")]
    [CounterCreationData(CounterType = ErrorRateType, Help = "Number of errors per second.")]
    public class ErrorRatePerformanceCounterAdapter : PerformanceCounterAdapterBase<
        ErrorRatePerformanceCounterAdapter>
    {
        internal ErrorRatePerformanceCounterAdapter(IPerformanceCounterDescriptor descriptor)
            : base(descriptor)
        {
        }

        internal const PerformanceCounterType ErrorRateType = PerformanceCounterType.RateOfCountsPerSecond64;

        private PerformanceCounter ErrorRateCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == ErrorRateType); }
        }

        public override void BeginMeasurement(IPerformanceCounterDescriptor descriptor)
        {
            // Nothing to do here.
        }

        public override void EndMeasurement(TimeSpan elapsed, IPerformanceCounterDescriptor descriptor)
        {
            ErrorRateCounter.Increment();
        }
    }
}
