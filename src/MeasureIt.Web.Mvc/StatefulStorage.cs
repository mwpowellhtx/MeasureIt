using System;
using System.Web;

namespace MeasureIt.Web.Mvc
{
    using Collections;

    internal class StatefulStorage : IStatefulStorage
    {
        private readonly IStatefulStorageDictionary _dictionary;

        private static IStatefulStorageDictionary CreateStatefulStorageDictionary(HttpContextBase context,
            StatefulStorageMode mode)
        {
            switch (mode)
            {
                case StatefulStorageMode.PerApplication:
                    return new ApplicationStatefulStorageDictionary(context);
                case StatefulStorageMode.PerRequest:
                    return new RequestStatefulStorageDictionary(context);
                case StatefulStorageMode.PerSession:
                    return new RequestStatefulStorageDictionary(context);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, $"{mode} is unsupported.");
            }
        }

        /// <summary>
        /// <see cref="StatefulStorageMode.PerRequest"/>
        /// </summary>
        private const StatefulStorageMode DefaultMode = StatefulStorageMode.PerRequest;

        internal StatefulStorage(HttpContextBase context, StatefulStorageMode mode = DefaultMode)
        {
            _dictionary = CreateStatefulStorageDictionary(context, mode);
        }

        public T Get<T>(string name)
        {
            return (T) _dictionary[name];
        }

        public T GetOrAdd<T>(string name, Func<T> factory)
        {
            if (!_dictionary.ContainsKey(name))
                _dictionary[name] = factory();
            return Get<T>(name);
        }

        public bool TryRemove(string name)
        {
            return _dictionary.Remove(name);
        }
    }
}
