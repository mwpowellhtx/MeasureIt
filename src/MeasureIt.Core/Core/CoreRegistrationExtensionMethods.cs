using System;

namespace MeasureIt
{
    /// <summary>
    /// Exposes some Core Registration extension methods.
    /// </summary>
    public static class CoreRegistrationExtensionMethods
    {
        /// <summary>
        /// Verifies whether <paramref name="type"/> <see cref="Type.IsInterface"/>.
        /// </summary>
        /// <param name="type"></param>
        public static void VerifyIsInterface(this Type type)
        {
            // Rule out whether TInterface is indeed an interface.
            if (type.IsInterface) return;

            var message = $"Requires an Interface type instead of {type.FullName}";

            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Verifies whether <paramref name="type"/> <see cref="Type.IsClass"/>.
        /// </summary>
        /// <param name="type"></param>
        public static void VerifyIsClass(this Type type)
        {
            // Rule out whether TInterface is indeed an interface.
            if (type.IsClass) return;

            var message = $"Requires an Class type instead of {type.FullName}";

            throw new InvalidOperationException(message);
        }
    }
}
