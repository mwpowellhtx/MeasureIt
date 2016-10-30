using System;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementContext : IContext
    {
        // TODO: TBD: Descriptor? or simply "options" ?
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        IPerformanceMeasurementDescriptor Descriptor { get; }

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
