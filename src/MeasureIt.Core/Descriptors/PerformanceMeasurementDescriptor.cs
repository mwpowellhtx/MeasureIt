using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Contexts;
    using Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptor
        : DescriptorBase
            , IPerformanceMeasurementDescriptor
            , IEquatable<PerformanceMeasurementDescriptor>
    {
        private string _prefix;

        private static string CalculatePrefix(string prefix, Type rootType, MethodInfo method)
        {
            const string defaultPrefix = "";

            // So we do not want DeclaringType or even ReflectedType here, but rather rootType.
            return !string.IsNullOrEmpty(prefix)
                ? prefix
                : (rootType == null || method == null
                    ? defaultPrefix
                    : string.Join(".", rootType.FullName, method.Name));
        }

        public string Prefix
        {
            get { return CalculatePrefix(_prefix, RootType, Method); }
            set { value.SetIf(out _prefix, string.Empty, s => !string.IsNullOrEmpty(s)); }
        }

        public bool ShouldIncludeCounterTypeInName { get; set; }

        private Type _categoryType;

        private static bool TryVerifyCategoryType(Type type, out string message)
        {
            message = null;

            if (!type.IsClass || type.IsAbstract)
            {
                // Occurs when Not a Class or when Abstract (i.e. Not Concrete).
                message = string.Format(@"Type {0} must be a concrete class.", type);
            }

            return string.IsNullOrEmpty(message);
        }

        public Type CategoryType
        {
            get { return _categoryType; }
            set
            {
                _categoryType = value.VerifyType<IPerformanceCounterCategoryAdapter>(TryVerifyCategoryType);

                //// TODO: TBD: leave "registering" or "unregistering" for the moment of discovery, discovery service(s), etc
                //UnregisterFromCategory(CategoryAdapter);
                //// TODO: TBD: the difficulty with a decoration/descriptor is that I'm not sure why we bother with a category adapter/attribute at all...
                //// TODO: I'm leaning towards removing it in simple favor of a "straight" hierarchy
                //var category = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);
                //RegisterWithCategory(CategoryAdapter = category);
            }
        }

        //private void RegisterWithCategory(IPerformanceCounterCategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Register(this);
        //}

        //private void UnregisterFromCategory(IPerformanceCounterCategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Unregister(this);
        //}

        /// <summary>
        /// Gets or sets the CategoryAdapter.
        /// </summary>
        public virtual IPerformanceCounterCategoryAdapter CategoryAdapter { get; set; }

        private IEnumerable<Type> _adapterTypes;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> AdapterTypes
        {
            get { return _adapterTypes; }
            private set
            {
                _adapterTypes = value.VerifyTypes<IPerformanceCounterAdapter>();

                foreach (var a in Adapters) a.Dispose();

                _adapters = CreateAdapters().ToList();
            }
        }

        private IList<IPerformanceCounterAdapter> _adapters;

        public IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _adapters; }
            private set { _adapters = (value ?? new List<IPerformanceCounterAdapter>()).ToList(); }
        }

        private IList<IPerformanceCounterAdapter> PrivateAdapters
        {
            get
            {
                return _adapters.ToBidirectionalList(
                    added => added.Measurement = this
                    , removed => removed.Measurement = null
                    );
            }
        }

        public bool? ReadOnly { get; set; }

        public PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        public bool PublishCounters { get; set; }

        public bool ThrowPublishErrors { get; set; }

        public bool PublishEvent { get; set; }

        /// <summary>
        /// Gets whether MayProceedUnabated.
        /// </summary>
        /// <see cref="PublishCounters"/>
        /// <see cref="PublishEvent"/>
        public bool MayProceedUnabated
        {
            get { return !(PublishCounters || PublishEvent); }
        }

        private double _sampleRate;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                const double min = Constants.MinSampleRate;
                const double max = Constants.MaxSampleRate;
                _sampleRate = Math.Max(min, Math.Min(max, value));
            }
        }

        public Type RootType { get; set; }

        private MethodInfo _method;

        public MethodInfo Method
        {
            get { return _method; }
            set
            {
                _method = value;
                Prefix = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementDescriptor(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : this(null, categoryType, adapterType, otherAdapterTypes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementDescriptor(string prefix, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            Initialize(prefix, categoryType, adapterType, otherAdapterTypes);
        }

        internal PerformanceMeasurementDescriptor(IPerformanceMeasurementDescriptor other)
        {
            Copy(other);
        }

        private void Initialize(string prefix, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            _adapters = new List<IPerformanceCounterAdapter>();

            Prefix = prefix;

            CategoryType = categoryType;
            AdapterTypes = new[] {adapterType}.Concat(otherAdapterTypes);

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        private void Copy(IPerformanceMeasurementDescriptor other)
        {
            _adapters = new List<IPerformanceCounterAdapter>();
            AdapterTypes = other.AdapterTypes.ToArray();

            CategoryType = other.CategoryType;
            CategoryAdapter = other.CategoryAdapter;

            Prefix = other.Prefix;
            InstanceLifetime = other.InstanceLifetime;

            RootType = other.RootType;
            Method = other.Method;

            PublishCounters = other.PublishCounters;
            PublishEvent = other.PublishEvent;
            ThrowPublishErrors = other.ThrowPublishErrors;

            SampleRate = other.SampleRate;

            ReadOnly = other.ReadOnly;
        }

        private IEnumerable<IPerformanceCounterAdapter> CreateAdapters()
        {
            // TODO: TBD: might do with relaying Options here...
            const BindingFlags publicNonPublicInstance
                = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var binder = Type.DefaultBinder;

            var ctors = AdapterTypes.Select(a => a.GetConstructor(publicNonPublicInstance,
                binder, new Type[0], null)).ToArray();

            if (!ctors.Any())
                throw new InvalidOperationException("Invalid descriptor instance.");

            var adapters = PrivateAdapters;

            foreach (IPerformanceCounterAdapter adapter in ctors.Select(ctor => ctor.Invoke(new object[0])))
            {
                adapters.Add(adapter);
                yield return adapter;
            }
        }

        public virtual IPerformanceMeasurementContext CreateContext()
        {
            return new PerformanceMeasurementContext(this, Adapters.ToArray());
        }

        //private IEnumerable<CounterCreationData> GetCounterCreationData()
        //{
        //    var prefix = Name;

        //    // It does not matter which order the Adapters themselves are turned in.
        //    foreach (var datum in Adapters.SelectMany(a => a.CreationData))
        //    {
        //        /* However, the CounterCreationData SHOULD be ordered properly. This is especially critical for
        //         * composite counters, which their Bases should appear IMMEDIATELY following their dependents. */

        //        var counterName = string.Join(".", prefix, datum.Name);
        //        yield return new CounterCreationData(counterName, datum.Help, datum.CounterType);
        //    }
        //}

        //// TODO: TBD: we want counter creation data for these? or as a wrapper/facade during installer context? that might make better sense...
        //public virtual IEnumerable<CounterCreationData> Data
        //{
        //    get { return GetCounterCreationData(); }
        //}

        /// <summary>
        /// Returns whether <paramref name="x"/> Equals <paramref name="y"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected static bool Equals(IPerformanceMeasurementDescriptor x,
            IPerformanceMeasurementDescriptor y)
        {
            // Rule the instances in or out based on these criteria.
            if (x == null || y == null) return false;

            if (ReferenceEquals(x, y)) return true;

            {
                var comparer = new MethodInfoEqualityComparer();

                // Ensure that the Method Base Definitions are in agreement.
                if (!comparer.Equals(x.Method.GetBaseDefinition(),
                    y.Method.GetBaseDefinition())) return false;
            }

            /* Cannot be considered if the Descriptors are not even related by lineage.
             * This is different from Similarity which requires only requires lineage. */

            if (x.RootType == null || y.RootType == null
                || x.RootType != y.RootType) return false;

            // Must be aligned with the same CategoryType.
            if (x.CategoryType == null || y.CategoryType == null
                || x.CategoryType != y.CategoryType) return false;

            {
                /* Last but not least ensure that the AdapterTypes are aligned.
                 * In my estimation, does not matter the order of the types, per se. */

                var comparer = new TypeEqualityComparer();

                return x.AdapterTypes.Count() == y.AdapterTypes.Count()
                       && x.AdapterTypes.OrderBy(t => t.FullName)
                           .Zip(y.AdapterTypes.OrderBy(t => t.FullName), (a, b) => new {a, b})
                           .All(z => comparer.Equals(z.a, z.b));
            }
        }

        public bool Equals(IPerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }

        public bool Equals(PerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }

        public override object Clone()
        {
            return new PerformanceMeasurementDescriptor(this);
        }
    }
}
