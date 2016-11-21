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

            var message = string.Format("Requires an Interface type instead of {0}", type.FullName);

            throw new InvalidOperationException(message);
        }
    }
}
