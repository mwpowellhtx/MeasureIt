using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeasureIt.Web.Mvc.Collections
{
    internal class RequestStatefulStorageDictionary : IRequestStatefulStorageDictionary
    {
        private readonly IDictionary _items;

        internal RequestStatefulStorageDictionary(HttpContextBase context)
        {
            _items = context.Items;
        }

        private static IEnumerable<string> GetKeys(IDictionary items)
        {
            return items.Keys.Cast<string>();
        }

        private void StatefulAction(Action<IDictionary> action)
        {
            action(_items);
        }

        private T StatefulFunc<T>(Func<IDictionary, T> func)
        {
            return func(_items);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return StatefulFunc(s => GetKeys(s).Select(k => new KeyValuePair<string, object>(k, s[k]))
                .GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            StatefulAction(s => s[item.Key] = item.Value);
        }

        public void Clear()
        {
            StatefulAction(s => s.Clear());
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return StatefulFunc(s => GetKeys(s).Contains(item.Key));
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return StatefulFunc(s =>
            {
                if (!GetKeys(s).Contains(item.Key))
                    return false;
                s.Remove(item.Key);
                return true;
            });
        }

        public int Count
        {
            get { return StatefulFunc(s => s.Count); }
        }

        public bool IsReadOnly
        {
            get { return StatefulFunc(s => false); }
        }

        public bool ContainsKey(string key)
        {
            return StatefulFunc(s => GetKeys(s).Contains(key));
        }

        public void Add(string key, object value)
        {
            StatefulAction(s => s[key] = value);
        }

        public bool Remove(string key)
        {
            return StatefulFunc(s =>
            {
                if (!GetKeys(s).Contains(key))
                    return false;
                s.Remove(key);
                return true;
            });
        }

        public bool TryGetValue(string key, out object value)
        {
            object local = null;
            var found = StatefulFunc(s =>
            {
                if (!GetKeys(s).Contains(key))
                    return false;
                local = s[key];
                return true;
            });
            value = local;
            return found;
        }

        public object this[string key]
        {
            get { return StatefulFunc(s => s[key]); }
            set { StatefulAction(s => s[key] = value); }
        }

        public ICollection<string> Keys => StatefulFunc(GetKeys).ToArray();

        public ICollection<object> Values
        {
            get { return StatefulFunc(s => GetKeys(s).Select(k => s[k])).ToArray(); }
        }
    }
}
