using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [PerformanceCounterAdapter("member activity timer")]
    [CounterCreationData(CounterType = ActivityTimerType, Help = "Measure of member activity in nanoseconds.")]
    public class MemberActivityTimerPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        MemberActivityTimerPerformanceCounterAdapter>
    {
        internal MemberActivityTimerPerformanceCounterAdapter(IMeasurePerformanceDescriptor descriptor)
            : base(descriptor)
        {
        }

        internal const PerformanceCounterType ActivityTimerType = PerformanceCounterType.Timer100Ns;

        private PerformanceCounter ActivityTimerCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == ActivityTimerType); }
        }

        public override void BeginMeasurement(IMeasurePerformanceDescriptor descriptor)
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

        public override void EndMeasurement(TimeSpan elapsed, IMeasurePerformanceDescriptor descriptor)
        {

            /* TODO: TBD: on the broader descriptor specification, "ReadOnly" does not make sense
             * from a measurement perspective; we WANT to be able to post changes, so by definition
             * they are NOT ReadOnly */

            ActivityTimerCounter.IncrementBy(CalculateElapsed100Ns(elapsed));
        }
    }
}
