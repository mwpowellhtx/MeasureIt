using System;
using System.Collections;
using System.Collections.Generic;

namespace MeasureIt.Collections.Generic
{
    /* TODO: TBD: was considering using something like this to facilitate connecting descriptors
     * with invocation, but decided against it... still, may prove useful at some point... However,
     * for this library's purposes, considering it obsolete and may archive the work apart from the
     * project solution eventually. */

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [Obsolete]
    public class Map<T1, T2> : IMap<T1, T2>
    {
        private readonly IDictionary<T1, T2> _leftDictionary;
        private readonly IDictionary<T2, T1> _rightDictionary;

        /// <summary>
        /// 
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftDictionary"></param>
        public Map(IDictionary<T1, T2> leftDictionary)
        {
            _leftDictionary = leftDictionary;
            _rightDictionary = leftDictionary.Mirror();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rightDictionary"></param>
        public Map(IDictionary<T2, T1> rightDictionary)
        {
            _rightDictionary = rightDictionary;
            _leftDictionary = rightDictionary.Mirror();
        }

        private static void DictionaryAction<T3, T4>(Action<IDictionary<T3, T4>> action,
            IDictionary<T3, T4> dictionary)
        {
            action(dictionary);
        }

        private static TResult DictionaryFunc<T3, T4, TResult>(Func<IDictionary<T3, T4>, TResult> func,
            IDictionary<T3, T4> dictionary)
        {
            return func(dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void Add(T1 left, T2 right)
        {
            DictionaryAction(l => l.Add(left, right), _leftDictionary);
            DictionaryAction(r => r.Add(right, left), _rightDictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        public void Add(T2 right, T1 left)
        {
            Add(left, right);
        }

        /// <summary>
        /// Returns whether the Map Contains the <paramref name="left"/> Key.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public bool ContainsKey(T1 left)
        {
            return DictionaryFunc(l => l.ContainsKey(left), _leftDictionary);
        }

        /// <summary>
        /// Returns whether the Map Contains the <paramref name="right"/> Key.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool ContainsKey(T2 right)
        {
            return DictionaryFunc(r => r.ContainsKey(right), _rightDictionary);
        }

        /// <summary>
        /// Gets the left Keys.
        /// </summary>
        public ICollection<T1> Keys
        {
            get { return DictionaryFunc(l => l.Keys, _leftDictionary); }
        }

        /// <summary>
        /// Gets the Mapped right Keys.
        /// </summary>
        public ICollection<T2> MappedKeys
        {
            get { return DictionaryFunc(r => r.Keys, _rightDictionary); }
        }

        /// <summary>
        /// Gets the left Values.
        /// </summary>
        public ICollection<T2> Values
        {
            get { return DictionaryFunc(l => l.Values, _leftDictionary); }
        }

        /// <summary>
        /// Gets the Mapped right Values.
        /// </summary>
        public ICollection<T1> MappedValues
        {
            get { return DictionaryFunc(r => r.Values, _rightDictionary); }
        }

        private static bool Remove<T3, T4>(T3 key, IDictionary<T3, T4> aDictionary,
            IDictionary<T4, T3> bDictionary)
        {
            return DictionaryFunc(a =>
            {
                var exists = a.ContainsKey(key);
                if (exists)
                {
                    DictionaryFunc(b => b.Remove(a[key]), bDictionary);
                }
                return a.Remove(key);
            }, aDictionary);
        }

        /// <summary>
        /// Removes the item corresponding to the <paramref name="left"/> Key.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public bool Remove(T1 left)
        {
            return Remove(left, _leftDictionary, _rightDictionary);
        }

        /// <summary>
        /// Removes the item corresponding to the <paramref name="right"/> Key.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool Remove(T2 right)
        {
            return Remove(right, _rightDictionary, _leftDictionary);
        }

        private static bool TryGetValue<T3, T4>(T3 key, out T4 value, IDictionary<T3, T4> dictionary)
        {
            var v = default(T4);
            var result = DictionaryFunc(l => l.TryGetValue(key, out v), dictionary);
            value = v;
            return result;
        }

        /// <summary>
        /// Tries to get the Value corresponding to the left <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(T1 key, out T2 value)
        {
            return TryGetValue(key, out value, _leftDictionary);
        }

        /// <summary>
        /// Tries to get the Value corresponding to the right <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(T2 key, out T1 value)
        {
            return TryGetValue(key, out value, _rightDictionary);
        }

        private static void IndexerSet<T3, T4>(T3 left, T4 right, IDictionary<T3, T4> leftDictionary,
            IDictionary<T4, T3> rightDictionary)
        {
            DictionaryAction(l => l[left] = right, leftDictionary);
            DictionaryAction(r => r[right] = left, rightDictionary);
        }

        /// <summary>
        /// Gets or sets the right value corresponding to the left <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T2 this[T1 key]
        {
            get { return DictionaryFunc(l => l[key], _leftDictionary); }
            set { IndexerSet(key, value, _leftDictionary, _rightDictionary); }
        }

        /// <summary>
        /// Gets or sets the left value corresponding to the right <paramref name="key"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T1 this[T2 key]
        {
            get { return DictionaryFunc(r => r[key], _rightDictionary); }
            set { IndexerSet(key, value, _rightDictionary, _leftDictionary); }
        }

        /// <summary>
        /// Adds the pair <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<T1, T2> item)
        {
            DictionaryAction(l => l.Add(item), _leftDictionary);
            DictionaryAction(r => r.Add(new KeyValuePair<T2, T1>(item.Value, item.Key)), _rightDictionary);
        }

        /// <summary>
        /// Adds the pair <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<T2, T1> item)
        {
            DictionaryAction(r => r.Add(item), _rightDictionary);
            DictionaryAction(r => r.Add(new KeyValuePair<T1, T2>(item.Value, item.Key)), _leftDictionary);
        }

        /// <summary>
        /// Clears the map.
        /// </summary>
        public void Clear()
        {
            DictionaryAction(l => l.Clear(), _leftDictionary);
            DictionaryAction(r => r.Clear(), _rightDictionary);
        }

        /// <summary>
        /// Returns whether the map Contains the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return DictionaryFunc(l => l.Contains(item), _leftDictionary);
        }

        /// <summary>
        /// Returns whether the map Contains the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<T2, T1> item)
        {
            return DictionaryFunc(r => r.Contains(item), _rightDictionary);
        }

        /// <summary>
        /// Gets the map Count.
        /// </summary>
        public int Count
        {
            get { return DictionaryFunc(l => l.Count, _leftDictionary); }
        }

        /// <summary>
        /// Gets whether the map IsReadOnly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return DictionaryFunc(l => l.IsReadOnly, _leftDictionary)
                       || DictionaryFunc(r => r.IsReadOnly, _rightDictionary);
            }
        }

        /// <summary>
        /// Removes the map corresponding to the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<T1, T2> item)
        {
            return DictionaryFunc(l => l.Remove(item), _leftDictionary)
                   || DictionaryFunc(r => r.Remove(new KeyValuePair<T2, T1>(item.Value, item.Key)), _rightDictionary);
        }

        /// <summary>
        /// Removes the map corresponding to the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<T2, T1> item)
        {
            return DictionaryFunc(r => r.Remove(item), _rightDictionary)
                   || DictionaryFunc(r => r.Remove(new KeyValuePair<T1, T2>(item.Value, item.Key)), _leftDictionary);
        }

        /// <summary>
        /// Copies the mapped items to the <paramref name="array"/> starting at the <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            DictionaryAction(l => l.CopyTo(array, arrayIndex), _leftDictionary);
        }

        /// <summary>
        /// Copies the mapped items to the <paramref name="array"/> starting at the <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex)
        {
            DictionaryAction(r => r.CopyTo(array, arrayIndex), _rightDictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return DictionaryFunc(l => l.GetEnumerator(), _leftDictionary);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
