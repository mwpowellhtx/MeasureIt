using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace MeasureIt
{
    using IExpandoObjectDictionary = IDictionary<string, object>;

    /* fulfills a man in the middle design pattern; performance counters are defined,
     * both installation/creation and runtime, on the front side, and on the back side
     * performance counter category adapter handles adapting into various reflected and
     * internet/web interception patterns */

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAdapter"></typeparam>
    public abstract class PerformanceCounterAdapterBase<TAdapter> : Disposable, IPerformanceCounterAdapter
        where TAdapter : PerformanceCounterAdapterBase<TAdapter>
    {
        /// <summary>
        /// 
        /// </summary>
        private class CounterCreationDataComparer : Comparer<CounterCreationData>
        {
            public override int Compare(CounterCreationData x, CounterCreationData y)
            {
                if (x == null && y == null) return 0;
                if (x != null && y == null) return 1;
                if (x == null) return -1;
                if (x.CounterType > y.CounterType) return -1;
                return x.CounterType < y.CounterType ? 1 : 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class PerformanceCounterComparer : Comparer<PerformanceCounter>
        {
            public override int Compare(PerformanceCounter x, PerformanceCounter y)
            {
                if (x == null && y == null) return 0;
                if (x != null && y == null) return 1;
                if (x == null) return -1;
                if (x.CounterType > y.CounterType) return -1;
                return x.CounterType < y.CounterType ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets the internal Parts of the Counter.
        /// </summary>
        protected internal ExpandoObject Parts { get; private set; }

        /// <summary>
        /// Returns whether <paramref name="dictionary"/> HasAny
        /// <see cref="IExpandoObjectDictionary.Values"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        protected static bool HasAny<T>(IExpandoObjectDictionary dictionary)
        {
            return dictionary.Values.OfType<T>().Any();
        }

        /// <summary>
        /// Returns All of the <see cref="IExpandoObjectDictionary.Values"/> of type
        /// <typeparamref name="T"/> matching <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TDictionary"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        private static IEnumerable<T> GetAll<TDictionary, T>(TDictionary dictionary, Func<T, bool> predicate = null)
            where TDictionary : IExpandoObjectDictionary
        {
            predicate = predicate ?? (x => true);
            return dictionary.Values.OfType<T>().Where(predicate);
        }

        private readonly IPerformanceCounterDescriptor _descriptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        protected PerformanceCounterAdapterBase(IPerformanceCounterDescriptor descriptor)
        {
            _descriptor = descriptor;
            Parts = new ExpandoObject();
        }

        private readonly CounterCreationDataComparer _dataComparer = new CounterCreationDataComparer();

        private readonly PerformanceCounterComparer _counterComparer = new PerformanceCounterComparer();

        private readonly Func<CounterCreationData, bool> _findAllData = x => true;

        private readonly Func<PerformanceCounter, bool> _findAllCounters = x => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parts"></param>
        protected virtual void InitializeData(ExpandoObject parts)
        {
            if (HasAny<CounterCreationData>(parts)) return;

            var adapter = _descriptor.AdapterDescriptor;

            // TODO: TBD: let's assume for the time being that the counter has already been named in an appropriate manner here...
            // TODO: TBD: if we need to provide some naming service/strategy, seems like we'd want to do it during discovery and/or in the agents
            var data = adapter.CreationDataDescriptors.Select(x => x.GetCounterCreationData());

            IExpandoObjectDictionary dictionary = parts;

            foreach (var datum in data) dictionary.Add(datum.CounterName, datum);
        }

        /// <summary>
        /// InitializeCounters
        /// </summary>
        protected virtual void InitializeCounters(ExpandoObject parts)
        {
            if (HasAny<PerformanceCounter>(parts)) return;

            var counters = _descriptor.GetPerformanceCounters();

            IExpandoObjectDictionary dictionary = parts;

            foreach (var c in counters) dictionary.Add(c.InstanceName, c);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<CounterCreationData> Data
        {
            get
            {
                InitializeData(Parts);
                return GetAll(Parts, _findAllData).OrderBy(x => x, _dataComparer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<PerformanceCounter> Counters
        {
            get
            {
                InitializeCounters(Parts);
                return GetAll(Parts, _findAllCounters).OrderBy(x => x, _counterComparer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        public abstract void BeginMeasurement(IPerformanceCounterDescriptor descriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public abstract void EndMeasurement(TimeSpan elapsed, IPerformanceCounterDescriptor descriptor);

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                foreach (var counter in Counters)
                {
                    counter.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
