using System;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// Provides some enumeration extension methods.
    /// </summary>
    public static class EnumExtensionMethods
    {
        /// <summary>
        /// Returns whether <paramref name="value"/> Contains Any of the <paramref name="others"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static bool Contains(this Enum value, params Enum[] others)
        {
            return others.Any(value.HasFlag);
        }

        /// <summary>
        /// Returns whether <paramref name="value"/> is ContainedBy Any of the <paramref name="others"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static bool ContainedBy(this Enum value, params Enum[] others)
        {
            return others.Any(o => o.HasFlag(value));
        }
    }
}
