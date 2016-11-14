using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Contexts;
    using ICounterAdapter = IPerformanceCounterAdapter;
    using ICategoryAdapter = IPerformanceCounterCategoryAdapter;
    using IMeasurementDescriptor = IPerformanceMeasurementDescriptor;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptor
        : DescriptorBase
            , IMeasurementDescriptor
            , IEquatable<PerformanceMeasurementDescriptor>
    {
        // TODO: TBD: what, if anything, to do about the RandomSeed?
        private IMoniker _nameMoniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new NameMoniker(name);
        }

        private static IMoniker GetMethodMoniker(MethodInfo method)
        {
            return method == null ? null : new MethodInfoMoniker(method);
        }

        public string Name
        {
            get { return _nameMoniker.ToString(); }
            set { _nameMoniker = GetNameMoniker(value) ?? GetMethodMoniker(Method) ?? DefaultMoniker.New(); }
        }

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
                _categoryType = value.VerifyType<ICategoryAdapter>(TryVerifyCategoryType);

                //// TODO: TBD: leave "registering" or "unregistering" for the moment of discovery, discovery service(s), etc
                //UnregisterFromCategory(CategoryAdapter);
                //// TODO: TBD: the difficulty with a decoration/descriptor is that I'm not sure why we bother with a category adapter/attribute at all...
                //// TODO: I'm leaning towards removing it in simple favor of a "straight" hierarchy
                //var category = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);
                //RegisterWithCategory(CategoryAdapter = category);
            }
        }

        //private void RegisterWithCategory(ICategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Register(this);
        //}

        //private void UnregisterFromCategory(ICategoryAdapter category)
        //{
        //    if (category == null) return;
        //    category.Unregister(this);
        //}

        /// <summary>
        /// Gets or sets the CategoryAdapter.
        /// </summary>
        public virtual ICategoryAdapter CategoryAdapter { get; set; }

        private IEnumerable<Type> _adapterTypes;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> AdapterTypes
        {
            get { return _adapterTypes; }
            set
            {
                _adapterTypes = value.VerifyTypes<ICounterAdapter>();

                foreach (var a in Adapters) a.Dispose();

                _adapters = CreateAdapters();
            }
        }

        private IEnumerable<ICounterAdapter> _adapters;

        public IEnumerable<ICounterAdapter> Adapters
        {
            get { return _adapters; }
            private set { _adapters = (value ?? new List<ICounterAdapter>()).ToArray(); }
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
                Name = null;
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
        /// <param name="name"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementDescriptor(string name, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            Initialize(name, categoryType, adapterType, otherAdapterTypes);
        }

        internal PerformanceMeasurementDescriptor(IMeasurementDescriptor other)
        {
            Copy(other);
        }

        private void Initialize(string name, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
        {
            Adapters = null;

            Name = name;

            CategoryType = categoryType;
            AdapterTypes = new[] {adapterType}.Concat(otherAdapterTypes);

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        private void Copy(IMeasurementDescriptor other)
        {
            Adapters = null;
            AdapterTypes = other.AdapterTypes.ToArray();

            CategoryType = other.CategoryType;
            CategoryAdapter = other.CategoryAdapter;

            Name = other.Name;
            InstanceLifetime = other.InstanceLifetime;

            RootType = other.RootType;
            Method = other.Method;

            PublishCounters = other.PublishCounters;
            PublishEvent = other.PublishEvent;
            ThrowPublishErrors = other.ThrowPublishErrors;

            SampleRate = other.SampleRate;

            ReadOnly = other.ReadOnly;
        }

        private IEnumerable<ICounterAdapter> CreateAdapters()
        {
            // TODO: TBD: might do with relaying Options here...
            const BindingFlags publicNonPublicInstance
                = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var binder = Type.DefaultBinder;

            var ctors = AdapterTypes.Select(a => a.GetConstructor(publicNonPublicInstance,
                binder, new Type[0], null)).ToArray();

            if (!ctors.Any())
                throw new InvalidOperationException("Invalid descriptor instance.");

            foreach (ICounterAdapter adapter in ctors.Select(ctor => ctor.Invoke(new object[0])))
            {
                // Make sure to connect the Adapter with This Measurement.
                adapter.Measurement = this;
                yield return adapter;
            }
        }

        public virtual IPerformanceMeasurementContext CreateContext()
        {
            return new PerformanceMeasurementContext(this, Adapters.ToArray());
        }

        private IEnumerable<CounterCreationData> GetCounterCreationData()
        {
            var prefix = Name;

            // It does not matter which order the Adapters themselves are turned in.
            foreach (var datum in Adapters.SelectMany(a => a.CreationData))
            {
                /* However, the CounterCreationData SHOULD be ordered properly. This is especially critical for
                 * composite counters, which their Bases should appear IMMEDIATELY following their dependents. */

                var counterName = string.Join(".", prefix, datum.Name);
                yield return new CounterCreationData(counterName, datum.Help, datum.CounterType);
            }
        }

        public virtual IEnumerable<CounterCreationData> Data
        {
            get { return GetCounterCreationData(); }
        }

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static bool Equals(IMeasurementDescriptor a, IMeasurementDescriptor b)
        {
            return ReferenceEquals(a, b)
                   || (
                       !(a.Method == null || b.Method == null)
                       && a.Method.GetBaseDefinition() == b.Method.GetBaseDefinition()
                       && !(a.RootType == null || b.RootType == null)
                       && a.RootType == b.RootType
                       && !(a.CategoryType == null || b.CategoryType == null
                            || a.AdapterTypes == null || b.AdapterTypes == null)
                       && a.CategoryType == b.CategoryType
                       && a.AdapterTypes.OrderBy(x => x.FullName)
                           .SequenceEqual(b.AdapterTypes.OrderBy(x => x.FullName))
                       );
        }

        public bool Equals(IMeasurementDescriptor other)
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
