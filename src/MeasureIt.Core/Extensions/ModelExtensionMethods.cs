using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public static class ModelExtensionMethods
    {
        /// <summary>
        /// Verifies that the <paramref name="type"/> is assignable to the
        /// <typeparamref name="TAssignableTo"/> type. <typeparamref name="TAssignableTo"/> may be
        /// anything, but is usually an interface.
        /// </summary>
        /// <typeparam name="TAssignableTo"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Type VerifyType<TAssignableTo>(this Type type)
        {
            var assignableToType = typeof(TAssignableTo);

            if (assignableToType.IsAssignableFrom(type))
            {
                return type;
            }

            var message = string.Format("The type {0} must be assignable to {1}."
                , type.FullName, assignableToType.FullName);

            throw new ArgumentException(message, "type");
        }

        /// <summary>
        /// Verifies that the <paramref name="types"/> are assignable to the
        /// <typeparamref name="TAssignableTo"/> type. <typeparamref name="TAssignableTo"/> may be
        /// anything, but is usually an interface.
        /// </summary>
        /// <typeparam name="TAssignableTo"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        internal static IEnumerable<Type> VerifyTypes<TAssignableTo>(this IEnumerable<Type> types)
        {
            return types.Select(VerifyType<TAssignableTo>);
        }

        internal static void IfNotNull(this IEnumerable<IPerformanceCounterAdapter> adapters
            , Action<IPerformanceCounterAdapter> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            adapters = adapters ?? new IPerformanceCounterAdapter[0];

            foreach (var adapter in adapters) action(adapter);
        }
    }
}
