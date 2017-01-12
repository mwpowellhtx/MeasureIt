using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = TimerType, Help = "Average time.")]
    [CounterCreationData(CounterType = BaseType, Help = "Average time (base).")]
    public class AverageTimePerformanceCounterAdapter : PerformanceCounterAdapterBase<
        AverageTimePerformanceCounterAdapter>
    {
        internal AverageTimePerformanceCounterAdapter()
        {
            // We need the Descriptor in order to convey necessary details to the Adapter.
        }

        internal const PerformanceCounterType TimerType = PerformanceCounterType.AverageTimer32;
        internal const PerformanceCounterType BaseType = PerformanceCounterType.AverageBase;

        private PerformanceCounter TimerCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == TimerType); }
        }

        private PerformanceCounter BaseCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == BaseType); }
        }

        // TODO: TBD: will need some sort of Descriptor capturing such contextual details as MethodInfo, I think

        // TODO: TBD: begin/end may not be a good pattern here; we may simple want to "take sample" ...

        /// <summary>
        /// Begins the Measurement.
        /// </summary>
        /// <param name="descriptor"></param>
        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do in this instance.
        }

        /// <summary>
        /// Ends the Measurement.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            TimerCounter.IncrementBy(elapsed.Ticks);
            BaseCounter.Increment();
        }
    }
}
