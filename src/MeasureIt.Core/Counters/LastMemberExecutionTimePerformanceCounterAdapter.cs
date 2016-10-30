﻿using System;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    [PerformanceCounterAdapter("last member execution time")]
    [CounterCreationData(CounterType = MemberAccessType, Help = "Last member execution time in milliseconds.")]
    public class LastMemberExecutionTimePerformanceCounterAdapter : PerformanceCounterAdapterBase<
        LastMemberExecutionTimePerformanceCounterAdapter>
    {
        internal LastMemberExecutionTimePerformanceCounterAdapter(IPerformanceMeasurementDescriptor descriptor)
            : base(descriptor)
        {
        }

        internal const PerformanceCounterType MemberAccessType = PerformanceCounterType.NumberOfItems64;

        private PerformanceCounter MemberAccessCounter
        {
            get { return Counters.SingleOrDefault(x => x.CounterType == MemberAccessType); }
        }

        public override void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor)
        {
            // Nothing to do here.
        }

        public override void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor)
        {
            MemberAccessCounter.RawValue = elapsed.Ticks;
        }
    }
}
