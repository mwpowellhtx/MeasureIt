using System;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    // TODO: TBD: potentially this could be re-factored in the Castle.Interception assembly
    /// <summary>
    /// 
    /// </summary>
    public interface IInterceptionMeasurementContext : IMeasurementContext
    {
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
