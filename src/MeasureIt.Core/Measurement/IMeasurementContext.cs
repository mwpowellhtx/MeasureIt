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
        IEnumerable<IPerformanceCounterContext> CounterContexts { get; }

        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        IPerformanceCounterDescriptor Descriptor { get; }

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
