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
        /// Begins the Measurement invocation.
        /// </summary>
        void BeginMeasurement();

        /// <summary>
        /// Ends the Measurement invocation.
        /// </summary>
        /// <param name="elapsed"></param>
        void EndMeasurement(TimeSpan elapsed);
    }
}
