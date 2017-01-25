using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MeasureIt
{
    using IRequestPropertiesDictionary = IDictionary<string, object>;

    internal static class MvcExtensionMethods
    {
        public static T GetService<T>(this IDependencyResolver resolver)
        {
            return (T) resolver.GetService(typeof(T));
        }

        public static IEnumerable<T> GetServices<T>(this IDependencyResolver resolver)
        {
            return resolver.GetServices(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Tries to <see cref="IDictionary{TKey,TValue}.Remove(TKey)"/> the item corresponding
        /// with <paramref name="key"/>. Tests whether
        /// <see cref="IDictionary{TKey,TValue}.ContainsKey"/> prior to removing.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryRemove(this IRequestPropertiesDictionary dictionary, string key)
        {
            return dictionary.ContainsKey(key) && dictionary.Remove(key);
        }
    }
}
