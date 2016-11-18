using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterAdapter : IDisposable
    {
        /// <summary>
        /// Gets or sets the Measurement.
        /// </summary>
        IPerformanceMeasurementDescriptor Measurement { get; set; }

        /// <summary>
        /// Gets the CreationData.
        /// </summary>
        IEnumerable<ICounterCreationDataDescriptor> CreationData { get; }

        // TODO: TBD: what to do about installation and/or runtime concerns: collection only? or receive a context?

        ///// <summary>
        ///// 
        ///// </summary>
        //IEnumerable<CounterCreationData> Data { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<PerformanceCounter> Counters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor);
    }
}
