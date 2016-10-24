using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeasureIt.Measurement
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementContext : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        Random Rnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IMeasurementOptions Options { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IPerformanceCounterContext> CounterContexts { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspect"></param>
        void Measure(Action aspect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspectGetter"></param>
        /// <returns></returns>
        Task MeasureAsync(Func<Task> aspectGetter);
    }
}
