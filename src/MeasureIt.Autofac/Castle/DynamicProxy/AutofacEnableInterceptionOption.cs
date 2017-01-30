using System;
using System.Linq;

namespace MeasureIt.Castle.DynamicProxy
{
    /// <summary>
    /// Signals whether to Enable Interception for <see cref="Class"/> or <see cref="Interface"/>
    /// or both.
    /// </summary>
    [Flags]
    public enum AutofacEnableInterceptionOption : int
    {
        /// <summary>
        /// Signals to Enable Interception for Class.
        /// </summary>
        Class = 1 << 0,

        /// <summary>
        /// Signals to Enable Interception for Interface.
        /// </summary>
        Interface = 1 << 1
    }

    internal static class AutofacEnableInterceptionOptionExtensionMethods
    {
        internal static bool TryContains(this AutofacEnableInterceptionOption value,
            params AutofacEnableInterceptionOption[] values)
        {
            return values.Any(x => (value & x) == x);
        }

        internal static bool TryDoesNotContain(this AutofacEnableInterceptionOption value,
            params AutofacEnableInterceptionOption[] values)
        {
            return !values.Any(x => (value & x) == x);
        }
    }
}
