using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [CounterCreationData(CounterType = AccessRateType, Help = "Number of member accesses per second.")]
    public class MemberAccessRatePerformanceCounterAdapter : PerformanceCounterAdapterBase<
        MemberAccessRatePerformanceCounterAdapter>
    {
        internal MemberAccessRatePerformanceCounterAdapter()
        {
        }

        internal const PerformanceCounterType AccessRateType = PerformanceCounterType.RateOfCountsPerSecond64;

        private PerformanceCounter AccessRateCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == AccessRateType); }
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
            // TODO: TBD: not sure that "access rate" or "number of operations per second" is quite what this captures, but I could be wrong...
            AccessRateCounter.Increment();
        }
    }
}
