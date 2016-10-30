using System;
using System.Reflection;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementProvider
    {
        // TODO: TBD: do we need any other arguments?
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method);
    }
}
