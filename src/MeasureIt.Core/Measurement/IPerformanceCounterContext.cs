using System;

namespace MeasureIt.Measurement
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterContext : IDisposable
    {
        // TODO: TBD: we need to wrap it in a Context? or just provide the adpater?
        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterAdapter Adapter { get; }

        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterDescriptor Descriptor { get; set; }
    }
}
