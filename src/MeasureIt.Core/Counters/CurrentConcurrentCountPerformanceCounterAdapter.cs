using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [PerformanceCounterAdapter("current concurrent count")]
    [CounterCreationData(CounterType = CountType, Help = "Number of requests running concurrently.")]
    public class CurrentConcurrentCountPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        CurrentConcurrentCountPerformanceCounterAdapter>
    {
        internal CurrentConcurrentCountPerformanceCounterAdapter(IPerformanceCounterDescriptor descriptor)
            : base(descriptor)
        {
        }

        internal const PerformanceCounterType CountType = PerformanceCounterType.NumberOfItems64;

        private PerformanceCounter CountCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == CountType); }
        }

        public override void BeginMeasurement(IPerformanceCounterDescriptor descriptor)
        {
            CountCounter.Increment();
        }

        public override void EndMeasurement(TimeSpan elapsed, IPerformanceCounterDescriptor descriptor)
        {
            CountCounter.Decrement();
        }
    }
}
