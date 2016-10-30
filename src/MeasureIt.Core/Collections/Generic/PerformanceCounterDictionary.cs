//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;

//namespace MeasureIt.Collections.Generic
//{
//    using IDictionaryType = IDictionary<Guid, PerformanceCounter>;
//    using DictionaryType = Dictionary<Guid, PerformanceCounter>;
//    using PerformanceCounterKeyValuePair = KeyValuePair<Guid, PerformanceCounter>;

//    internal class PerformanceCounterDictionary : IPerformanceCounterDictionary
//    {
//        private readonly Lazy<IDictionaryType> _lazyCounters;

//        private IDictionaryType Counters
//        {
//            get { return _lazyCounters.Value; }
//        }

//        private static IDictionaryType CreateDictionary(
//            IMeasurePerformanceDescriptor performanceDescriptor
//            , IEnumerable<ICounterCreationDataDescriptor> dataDescriptors)
//        {
//            IDictionaryType results = new DictionaryType();

//            var prefix = performanceDescriptor.Name;

//            var categoryName = performanceDescriptor.CategoryDescriptor.Name;
//            var readOnly = performanceDescriptor.ReadOnly;

//            foreach (var datum in dataDescriptors)
//            {
//                var moniker = new DefaultMoniker();

//                var counterName = string.Join(".", prefix, datum.Name);

//                // ReSharper disable once SwitchStatementMissingSomeCases
//                switch (readOnly)
//                {
//                    case true:
//                    case false:
//                        results[moniker.Id] = new PerformanceCounter(categoryName, counterName, moniker.ToString(), readOnly.Value);
//                        break;

//                    default:
//                        results[moniker.Id] = new PerformanceCounter(categoryName, counterName, moniker.ToString());
//                        break;
//                }
//            }

//            return results;
//        }

//        internal PerformanceCounterDictionary(
//            IMeasurePerformanceDescriptor performanceDescriptor
//            , IEnumerable<ICounterCreationDataDescriptor> dataDescriptors)
//        {
//            _lazyCounters = new Lazy<IDictionaryType>(
//                () => CreateDictionary(performanceDescriptor, dataDescriptors));
//        }

//        private void DictionaryAction(Action<IDictionaryType> action)
//        {
//            action(Counters);
//        }

//        private TResult DictionaryFunc<TResult>(Func<IDictionaryType, TResult> func)
//        {
//            return func(Counters);
//        }

//        public void Add(Guid key, PerformanceCounter value)
//        {
//            DictionaryAction(d => d.Add(key, value));
//        }

//        public bool ContainsKey(Guid key)
//        {
//            return DictionaryFunc(d => d.ContainsKey(key));
//        }

//        public ICollection<Guid> Keys
//        {
//            get { return DictionaryFunc(d => d.Keys); }
//        }

//        public bool Remove(Guid key)
//        {
//            return DictionaryFunc(d => d.Remove(key));
//        }

//        public bool TryGetValue(Guid key, out PerformanceCounter value)
//        {
//            var x = default(PerformanceCounter);
//            var result = DictionaryFunc(d => d.TryGetValue(key, out x));
//            return ((value = x) != null) && result;
//        }

//        public ICollection<PerformanceCounter> Values
//        {
//            get { return DictionaryFunc(d => d.Values); }
//        }

//        public PerformanceCounter this[Guid key]
//        {
//            get { return DictionaryFunc(d => d[key]); }
//            set { DictionaryAction(d => d[key] = value); }
//        }

//        public void Add(PerformanceCounterKeyValuePair item)
//        {
//            DictionaryAction(d => d.Add(item));
//        }

//        public void Clear()
//        {
//            DictionaryAction(d => d.Clear());
//        }

//        public bool Contains(PerformanceCounterKeyValuePair item)
//        {
//            return DictionaryFunc(d => d.Contains(item));
//        }

//        public void CopyTo(PerformanceCounterKeyValuePair[] array, int arrayIndex)
//        {
//            DictionaryAction(d => d.CopyTo(array, arrayIndex));
//        }

//        public int Count
//        {
//            get { return DictionaryFunc(d => d.Count); }
//        }

//        public bool IsReadOnly
//        {
//            get { return DictionaryFunc(d => d.IsReadOnly); }
//        }

//        public bool Remove(PerformanceCounterKeyValuePair item)
//        {
//            return DictionaryFunc(d => d.Remove(item));
//        }

//        public IEnumerator<PerformanceCounterKeyValuePair> GetEnumerator()
//        {
//            return DictionaryFunc(d => d.GetEnumerator());
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//    }
//}
