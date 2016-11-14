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
        /// Tries to Verify the <paramref name="type"/>, providing a
        /// <paramref name="failureMessage"/> upon failure.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public delegate bool TryVerifyTypeDelegate(Type type, out string failureMessage);

        private static bool DefaultTryVerifyType(Type type, out string failureMessage)
        {
            failureMessage = null;
            return true;
        }

        /// <summary>
        /// Verifies that the <paramref name="type"/> is assignable to the
        /// <typeparamref name="TAssignableTo"/> type. <typeparamref name="TAssignableTo"/> may be
        /// anything, but is usually an interface.
        /// </summary>
        /// <typeparam name="TAssignableTo"></typeparam>
        /// <param name="type"></param>
        /// <param name="tryVerify"></param>
        /// <returns></returns>
        internal static Type VerifyType<TAssignableTo>(this Type type, TryVerifyTypeDelegate tryVerify = null)
        {
            tryVerify = tryVerify ?? DefaultTryVerifyType;

            var assignableToType = typeof(TAssignableTo);

            string message;

            if (!assignableToType.IsAssignableFrom(type))
            {
                message = string.Format("The type {0} must be assignable to {1}."
                    , type.FullName, assignableToType.FullName);
                throw new ArgumentException(message, "type");
            }

            if (!tryVerify(type, out message))
            {
                throw new ArgumentException(message);
            }

            return type;
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
            return types.Select(t => VerifyType<TAssignableTo>(t));
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
