using System;
using System.Reflection;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITwoStageMeasurementProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        ITwoStageMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method);
    }
}
