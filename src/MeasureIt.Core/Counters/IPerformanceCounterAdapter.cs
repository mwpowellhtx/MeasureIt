using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterAdapter : IDisposable
    {
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        IPerformanceCounterAdapterDescriptor Descriptor { get; }

        // TODO: TBD: what to do about installation and/or runtime concerns: collection only? or receive a context?

        ///// <summary>
        ///// 
        ///// </summary>
        //IEnumerable<CounterCreationData> Data { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        //IEnumerable<PerformanceCounter> Counters { get; }

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
