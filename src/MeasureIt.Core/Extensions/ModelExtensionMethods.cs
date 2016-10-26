using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelExtensionMethods
    {
        internal static Type VerifyAdapterType(this Type adapterType)
        {
            var interfaceType = typeof(IPerformanceCounterAdapter);

            // ReSharper disable once InvertIf
            if (!interfaceType.IsAssignableFrom(adapterType))
            {
                var message = string.Format("The adapter type must implement {0}.", interfaceType.FullName);
                throw new ArgumentException(message, "adapterType");
            }

            return adapterType;
        }

        internal static void IfNotNull(this IPerformanceCounterAdapter adapter,
            Action<IPerformanceCounterAdapter> action)
        {
            if (adapter == null) return;
            action(adapter);
        }
    }
}
