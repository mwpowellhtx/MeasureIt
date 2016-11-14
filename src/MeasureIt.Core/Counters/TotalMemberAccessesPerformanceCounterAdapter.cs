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
            : base("total member accesses", "Total number of member accesses.")
        {
        }

        internal const PerformanceCounterType MemberAccessesType = PerformanceCounterType.NumberOfItems64;

        private PerformanceCounter MemberAccessesCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == MemberAccessesType); }
        }

        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do here.
        }

        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            MemberAccessesCounter.Increment();
        }
    }
}
