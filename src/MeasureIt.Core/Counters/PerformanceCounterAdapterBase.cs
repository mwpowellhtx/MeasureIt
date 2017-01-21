using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace MeasureIt
{
    using Adapters;
    using Collections.Generic;

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
    public abstract class PerformanceCounterAdapterBase<TAdapter> : Disposable
            , IPerformanceCounterAdapter
        where TAdapter : PerformanceCounterAdapterBase<TAdapter>
    {
        /// <summary>
        /// Returns the Calculated <see cref="Name"/>.
        /// </summary>
        /// <returns></returns>
        private string CalculateAdapterName()
        {
            var type = GetType();
            const string performanceCounterAdapterSuffix = "PerformanceCounterAdapter";
            return type.Name.Substring(0, type.Name.Length - performanceCounterAdapterSuffix.Length);
        }

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public virtual string Name
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return CalculateAdapterName(); }
        }

        /// <summary>
        /// Gets or sets the Measurement corresponding with the Adapter.
        /// </summary>
        public virtual IPerformanceMeasurementDescriptor Measurement { get; set; }

        private readonly IList<ICounterCreationDataDescriptor> _creationData;

        /// <summary>
        /// Gets the CreationData descriptors corresponding with the Adapter.
        /// </summary>
        public virtual IEnumerable<ICounterCreationDataDescriptor> CreationData => _creationData;

        /// <summary>
        /// Gets the <see cref="CreationData"/> for private use.
        /// </summary>
        private IList<ICounterCreationDataDescriptor> PrivateCreationData
        {
            get
            {
                return _creationData.ToBidirectionalList(
                    added => added.Adapter = this
                    , removed => removed.Adapter = null
                    );
            }
        }

        /// <summary>
        /// Gets the internal Parts of the Counter.
        /// </summary>
        protected internal ExpandoObject Parts { get; }

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
        {
            Parts = new ExpandoObject();

            /* Be careful of initialization: CounterCreationData order is CRITICAL. We do not
             * care what the attributes say, per se, but we do want the results ordered. */

            _creationData = new List<ICounterCreationDataDescriptor>();

            var type = GetType();

            var creationData = PrivateCreationData;

            foreach (var datum in type.GetAttributeValues((CounterCreationDataAttribute a) =>
                (ICounterCreationDataDescriptor) a.Descriptor.Clone()).OrderBy(x => x.CounterType))
            {
                creationData.Add(datum);
            }
        }

        private readonly Func<PerformanceCounter, bool> _findAllCounters = x => true;

        private static IEnumerable<PerformanceCounter> CreatePerformanceCounters(
            IPerformanceMeasurementDescriptor measurement
            , IEnumerable<ICounterCreationDataDescriptor> dataDescriptors)
        {
            var categoryName = measurement.CategoryAdapter.Name.PrepareCategoryName();
            var readOnly = measurement.ReadOnly;

            foreach (var datum in dataDescriptors)
            {
                var instanceName = Guid.NewGuid().ToString("N");

                /* TODO: TBD: not sure why, but not all of the ctor's work without throwing InvalidOperationException
                 * to do with "Could not locate Performance Counter with specified category name ...". */

                // However, using an initializer list seems to be just fine.
                var counter = new PerformanceCounter
                {
                    CategoryName = categoryName,
                    CounterName = datum.Name,
                    InstanceLifetime = measurement.InstanceLifetime,
                    InstanceName = instanceName,
                    ReadOnly = readOnly ?? false
                };

                yield return counter;
            }
        }

        // TODO: TBD: whereas initializing Counters is an "individual" MEASUREPERFORMANCE concern...
        /// <summary>
        /// Initializes the Performance Counters.
        /// </summary>
        protected virtual void InitializePerformanceCounters(ExpandoObject parts)
        {
            IExpandoObjectDictionary dictionary = parts;

            const string countersInitialized = "_countersInitialized";

            if (dictionary.ContainsKey(countersInitialized)
                && (bool) dictionary[countersInitialized])
            {
                return;
            }

            //// TODO: TBD: could do this in lieu of or in addition to, but let's not complicate matters...
            //if (HasAny<PerformanceCounter>(parts)) return;

            var counters = CreatePerformanceCounters(Measurement, CreationData);

            foreach (var c in counters) dictionary.Add(c.InstanceName, c);

            dictionary[countersInitialized] = true;
        }

        /// <summary>
        /// Gets the Counters corresponding with the Adapter.
        /// </summary>
        public IEnumerable<PerformanceCounter> Counters
        {
            get
            {
                InitializePerformanceCounters(Parts);
                return GetAll(Parts, _findAllCounters).OrderBy(x => x.CounterType);
            }
        }

        /// <summary>
        /// Begins the Measurement.
        /// </summary>
        /// <param name="descriptor"></param>
        public abstract void BeginMeasurement(IPerformanceMeasurementDescriptor descriptor);

        /// <summary>
        /// Ends the Measurement.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <param name="descriptor"></param>
        public abstract void EndMeasurement(TimeSpan elapsed, IPerformanceMeasurementDescriptor descriptor);

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
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
