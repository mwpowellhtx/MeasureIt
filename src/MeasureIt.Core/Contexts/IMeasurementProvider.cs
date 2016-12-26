using System;
using System.Reflection;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparamref name="TContext"/>
    public interface IMeasurementProvider<out TContext>
        where TContext : class, IMeasurementContext
    {
        // TODO: TBD: do we need any other arguments?
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        TContext GetMeasurementContext(Type targetType, MethodInfo method);
    }
}
