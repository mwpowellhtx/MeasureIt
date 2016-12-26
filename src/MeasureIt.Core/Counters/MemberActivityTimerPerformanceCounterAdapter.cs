using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = ActivityTimerType, Help = "Measure of member activity in nanoseconds.")]
    public class MemberActivityTimerPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        MemberActivityTimerPerformanceCounterAdapter>
    {
        internal MemberActivityTimerPerformanceCounterAdapter()
        {
        }

        internal const PerformanceCounterType ActivityTimerType = PerformanceCounterType.Timer100Ns;

        private PerformanceCounter ActivityTimerCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == ActivityTimerType); }
        }

        /// <summary>
        /// Begins the Measurement.
        /// </summary>
        /// <param name="descriptor"></param>
        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do here.
        }

        private static long CalculateElapsed100Ns(TimeSpan elapsed)
        {
            const long nanoSecondsPerSecond = 1000000000;
            const long oneHundred = 100;
            var seconds = elapsed.Ticks/Stopwatch.Frequency;
            return seconds*(nanoSecondsPerSecond/oneHundred);
        }

        /// <summary>
        /// Ends the Measurement.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {

            /* TODO: TBD: on the broader descriptor specification, "ReadOnly" does not make sense
             * from a measurement perspective; we WANT to be able to post changes, so by definition
             * they are NOT ReadOnly */

            ActivityTimerCounter.IncrementBy(CalculateElapsed100Ns(elapsed));
        }
    }
}
