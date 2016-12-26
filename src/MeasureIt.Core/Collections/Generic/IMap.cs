using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IMap<T1, T2> : IDictionary<T1, T2>
    {
        /// <summary>
        /// 
        /// </summary>
        ICollection<T2> MappedKeys { get; }

        /// <summary>
        /// 
        /// </summary>
        ICollection<T1> MappedValues { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="left"></param>
        ///// <param name="right"></param>
        //void Add(T1 left, T2 right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <param name="left"></param>
        void Add(T2 right, T1 left);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="left"></param>
        ///// <returns></returns>
        //bool ContainsKey(T1 left);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        bool ContainsKey(T2 right);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="left"></param>
        ///// <returns></returns>
        //bool Remove(T1 left);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        bool Remove(T2 right);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //bool TryGetValue(T1 key, out T2 value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGetValue(T2 key, out T1 value);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="left"></param>
        ///// <returns></returns>
        //T2 this[T1 left] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        T1 this[T2 right] { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="item"></param>
        //void Add(KeyValuePair<T1, T2> item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void Add(KeyValuePair<T2, T1> item);

        ///// <summary>
        ///// 
        ///// </summary>
        //void Clear();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //bool Contains(KeyValuePair<T1, T2> item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(KeyValuePair<T2, T1> item);

        ///// <summary>
        ///// 
        ///// </summary>
        //int Count { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        //bool IsReadOnly { get; }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //bool Remove(KeyValuePair<T1, T2> item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove(KeyValuePair<T2, T1> item);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="array"></param>
        ///// <param name="arrayIndex"></param>
        //void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void CopyTo(KeyValuePair<T2, T1>[] array, int arrayIndex);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //IEnumerator<KeyValuePair<T1, T2>> GetEnumerator();
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MapExtensionMethods
    {
        /// <summary>
        /// Returns the Mirror image of the <paramref name="dictionary"/>.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        internal static IDictionary<T2, T1> Mirror<T1, T2>(this IDictionary<T1, T2> dictionary)
        {
            return dictionary.ToDictionary(x => x.Value, x => x.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IMap<T1, T2> ToMap<T1, T2>(this IDictionary<T1, T2> dictionary)
        {
            return new Map<T1, T2>(dictionary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IMap<T1, T2> ToMap<T1, T2>(this IDictionary<T2, T1> dictionary)
        {
            return new Map<T1, T2>(dictionary);
        }
    }
}
