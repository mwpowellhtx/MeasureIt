using System;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceMeasurementContext : IDisposable
    {
        // TODO: TBD: we need to wrap it in a Context? or just provide the adpater?

        /// <summary>
        /// Begins the Performance Measurement invocation.
        /// </summary>
        void BeginMeasurement();

        /// <summary>
        /// Ends the Performance Measurement invocation.
        /// </summary>
        /// <param name="elapsed"></param>
        void EndMeasurement(TimeSpan elapsed);
    }
}
