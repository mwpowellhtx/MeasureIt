using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = MemberAccessesType, Help = "Total number of member accesses.")]
    public class TotalMemberAccessesPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        TotalMemberAccessesPerformanceCounterAdapter>
    {
        internal TotalMemberAccessesPerformanceCounterAdapter()
        {
        }

        internal const PerformanceCounterType MemberAccessesType = PerformanceCounterType.NumberOfItems64;

        private PerformanceCounter MemberAccessesCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == MemberAccessesType); }
        }

        /// <summary>
        /// Begins the Measurement.
        /// </summary>
        /// <param name="descriptor"></param>
        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do here.
        }

        /// <summary>
        /// Ends the Measurement.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            MemberAccessesCounter.Increment();
        }
    }
}
