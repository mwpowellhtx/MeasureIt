using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = CountType, Help = "Number of requests running concurrently.")]
    public class CurrentConcurrentCountPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        CurrentConcurrentCountPerformanceCounterAdapter>
    {
        internal CurrentConcurrentCountPerformanceCounterAdapter()
        {
        }

        internal const PerformanceCounterType CountType = PerformanceCounterType.NumberOfItems64;

        private PerformanceCounter CountCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == CountType); }
        }

        /// <summary>
        /// Begins the Measururement.
        /// </summary>
        /// <param name="descriptor"></param>
        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            CountCounter.Increment();
        }

        /// <summary>
        /// Ends the Measurement.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            CountCounter.Decrement();
        }
    }
}
