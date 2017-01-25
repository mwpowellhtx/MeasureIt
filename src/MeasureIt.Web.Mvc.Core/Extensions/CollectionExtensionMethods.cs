using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    internal static class CollectionExtensionMethods
    {
        /// <summary>
        /// Returns that None of the <paramref name="values"/> meet the conditions described
        /// in the <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal static bool None<T>(this IEnumerable<T> values, Func<T, bool> predicate)
        {
            // Return meaning the are Not Any Predicates aligning with the criteria.
            return !values.Any(predicate);
        }
    }
}
