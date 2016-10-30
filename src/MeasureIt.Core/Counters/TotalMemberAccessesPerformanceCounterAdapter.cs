using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [PerformanceCounterAdapter("total member accesses", Help = "Total number of member accesses.")]
    [CounterCreationData(CounterType = MemberAccessesType, Help = "Total number of member accesses.")]
    public class TotalMemberAccessesPerformanceCounterAdapter : PerformanceCounterAdapterBase<
        TotalMemberAccessesPerformanceCounterAdapter>
    {
        internal TotalMemberAccessesPerformanceCounterAdapter(IPerformanceMeasurementDescriptor descriptor)
            : base(descriptor)
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
