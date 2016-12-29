using System;

namespace MeasureIt.Web.Mvc
{
    internal interface IStatefulStorage
    {
        /// <summary>
        /// Returns the instance of the typed value by <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name);

        /// <summary>
        /// Returns the instance of the typed value by <paramref name="name"/>. If the value
        /// cannot be found, creates a new one by the <paramref name="factory"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        T GetOrAdd<T>(string name, Func<T> factory);

        /// <summary>
        /// Tries to Remove the item corresponding to the <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool TryRemove(string name);
    }
}
