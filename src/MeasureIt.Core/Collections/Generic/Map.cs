using System;
using System.Collections;
using System.Collections.Generic;

namespace MeasureIt.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
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

        public bool ContainsKey(T1 left)
        {
            return DictionaryFunc(l => l.ContainsKey(left), _leftDictionary);
        }

        public bool ContainsKey(T2 right)
        {
            return DictionaryFunc(r => r.ContainsKey(right), _rightDictionary);
        }

        public ICollection<T1> Keys
        {
            get { return DictionaryFunc(l => l.Keys, _leftDictionary); }
        }

        public ICollection<T2> MappedKeys
        {
            get { return DictionaryFunc(r => r.Keys, _rightDictionary); }
        }

        public ICollection<T2> Values
        {
            get { return DictionaryFunc(l => l.Values, _leftDictionary); }
        }

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

        public bool Remove(T1 left)
        {
            return Remove(left, _leftDictionary, _rightDictionary);
        }

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

        public bool TryGetValue(T1 key, out T2 value)
        {
            return TryGetValue(key, out value, _leftDictionary);
        }

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

        public T2 this[T1 key]
        {
            get { return DictionaryFunc(l => l[key], _leftDictionary); }
            set { IndexerSet(key, value, _leftDictionary, _rightDictionary); }
        }

        public T1 this[T2 key]
        {
            get { return DictionaryFunc(r => r[key], _rightDictionary); }
            set { IndexerSet(key, value, _rightDictionary, _leftDictionary); }
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            DictionaryAction(l => l.Add(item), _leftDictionary);
            DictionaryAction(r => r.Add(new KeyValuePair<T2, T1>(item.Value, item.Key)), _rightDictionary);
        }

        public void Add(KeyValuePair<T2, T1> item)
        {
            DictionaryAction(r => r.Add(item), _rightDictionary);
            DictionaryAction(r => r.Add(new KeyValuePair<T1, T2>(item.Value, item.Key)), _leftDictionary);
        }

        public void Clear()
        {
            DictionaryAction(l => l.Clear(), _leftDictionary);
            DictionaryAction(r => r.Clear(), _rightDictionary);
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return DictionaryFunc(l => l.Contains(item), _leftDictionary);
        }

        public bool Contains(KeyValuePair<T2, T1> item)
        {
            return DictionaryFunc(r => r.Contains(item), _rightDictionary);
        }

        public int Count
        {
            get { return DictionaryFunc(l => l.Count, _leftDictionary); }
        }

        public bool IsReadOnly
        {
            get
            {
                return DictionaryFunc(l => l.IsReadOnly, _leftDictionary)
                       || DictionaryFunc(r => r.IsReadOnly, _rightDictionary);
            }
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            return DictionaryFunc(l => l.Remove(item), _leftDictionary)
                   || DictionaryFunc(r => r.Remove(new KeyValuePair<T2, T1>(item.Value, item.Key)), _rightDictionary);
        }

        public bool Remove(KeyValuePair<T2, T1> item)
        {
            return DictionaryFunc(r => r.Remove(item), _rightDictionary)
                   || DictionaryFunc(r => r.Remove(new KeyValuePair<T1, T2>(item.Value, item.Key)), _leftDictionary);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            DictionaryAction(l => l.CopyTo(array, arrayIndex), _leftDictionary);
        }

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
