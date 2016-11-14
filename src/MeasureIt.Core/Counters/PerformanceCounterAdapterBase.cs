﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace MeasureIt
{
    using IExpandoObjectDictionary = IDictionary<string, object>;
    using IPerformanceCounterDictionary = IDictionary<Guid, PerformanceCounter>;
    using PerformanceCounterDictionary = Dictionary<Guid, PerformanceCounter>;

    /* fulfills a man in the middle design pattern; performance counters are defined,
     * both installation/creation and runtime, on the front side, and on the back side
     * performance counter category adapter handles adapting into various reflected and
     * internet/web interception patterns */

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAdapter"></typeparam>
    public abstract class PerformanceCounterAdapterBase<TAdapter> : NamedDisposable, IPerformanceCounterAdapter
        where TAdapter : PerformanceCounterAdapterBase<TAdapter>
    {
        public virtual IPerformanceMeasurementDescriptor Measurement { get; set; }

        public virtual string Help { get; set; }

        private readonly Lazy<IEnumerable<ICounterCreationDataDescriptor>> _lazyCreationData;

        public virtual IEnumerable<ICounterCreationDataDescriptor> CreationData
        {
            get { return _lazyCreationData.Value; }
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

        /// <summary>
        /// Protected Constructor
        /// </summary>
        protected PerformanceCounterAdapterBase()
            : this(null)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="help"></param>
        protected PerformanceCounterAdapterBase(string name, string help = null)
            : base(name)
        {
            Initialize(help);

            Parts = new ExpandoObject();

            var type = GetType();

            _lazyCreationData = new Lazy<IEnumerable<ICounterCreationDataDescriptor>>(() => type
                .GetAttributeValues((CounterCreationDataAttribute a) =>
                {
                    var cloned = (ICounterCreationDataDescriptor) a.Descriptor.Clone();
                    cloned.Adapter = this;
                    return cloned;
                }).ToArray());
        }

        private void Initialize(string help)
        {
            Help = help ?? string.Empty;
        }

        private readonly PerformanceCounterComparer _counterComparer = new PerformanceCounterComparer();

        private readonly Func<CounterCreationData, bool> _findAllData = x => true;

        private readonly Func<PerformanceCounter, bool> _findAllCounters = x => true;

        private static IEnumerable<PerformanceCounter> CreatePerformanceCounters(
            IPerformanceMeasurementDescriptor measurement
            , IEnumerable<ICounterCreationDataDescriptor> dataDescriptors)
        {
            var prefix = measurement.Name;

            var categoryName = measurement.CategoryAdapter.Name;
            var readOnly = measurement.ReadOnly;

            foreach (var datum in dataDescriptors)
            {
                var moniker = new DefaultMoniker();

                var instanceName = moniker.ToString();

                var counterName = string.Join(".", prefix, datum.Name);

                yield return new PerformanceCounter(categoryName, counterName, instanceName, readOnly ?? false)
                {
                    InstanceLifetime = measurement.InstanceLifetime
                };
            }
        }

        // TODO: TBD: whereas initializing Counters is an "individual" MEASUREPERFORMANCE concern...
        /// <summary>
        /// 
        /// </summary>
        protected virtual void InitializePerformanceCounters(ExpandoObject parts)
        {
            if (HasAny<PerformanceCounter>(parts)) return;

            var counters = CreatePerformanceCounters(Measurement, CreationData);

            IExpandoObjectDictionary dictionary = parts;

            foreach (var c in counters) dictionary.Add(c.InstanceName, c);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<PerformanceCounter> Counters
        {
            get
            {
                InitializePerformanceCounters(Parts);
                return GetAll(Parts, _findAllCounters).OrderBy(x => x, _counterComparer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        public abstract void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public abstract void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor);

        protected override void Dispose(bool disposing)
        {
            // TODO: TBD: what to do about disposal here?
            if (!IsDisposed && disposing)
            {
                //foreach (var counter in Counters)
                //{
                //    counter.Dispose();
                //}
            }

            base.Dispose(disposing);
        }
    }
}
