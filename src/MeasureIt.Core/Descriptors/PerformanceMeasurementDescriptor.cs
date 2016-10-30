using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Contexts;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementDescriptor : DescriptorBase
        , IPerformanceMeasurementDescriptor
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

        public IPerformanceCounterCategoryDescriptor CategoryDescriptor { get; set; }

        private Type _categoryType;

        public Type CategoryType
        {
            get { return _categoryType; }
            set
            {
                _categoryType = value.VerifyType<IPerformanceCounterCategoryAdapter>();

                UnregisterFromCategory(CategoryDescriptor);

                var category = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);

                RegisterWithCategory(CategoryDescriptor = category);
            }
        }

        private void RegisterWithCategory(IPerformanceCounterCategoryDescriptor category)
        {
            if (category == null) return;
            category.Register(this);
        }

        private void UnregisterFromCategory(IPerformanceCounterCategoryDescriptor category)
        {
            if (category == null) return;
            category.Unregister(this);
        }

        private IEnumerable<Type> _adapterTypes;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Type> AdapterTypes
        {
            get { return _adapterTypes; }
            set
            {
                _adapterTypes = value.VerifyTypes<IPerformanceCounterAdapter>();

                foreach (var a in Adapters) a.Dispose();

                _lazyAdapters = new Lazy<IEnumerable<IPerformanceCounterAdapter>>(CreateAdapters);
            }
        }

        private Lazy<IEnumerable<IPerformanceCounterAdapter>> _lazyAdapters;

        public IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _lazyAdapters.Value; }
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
        public PerformanceMeasurementDescriptor(string name, Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
        {
            _lazyAdapters = new Lazy<IEnumerable<IPerformanceCounterAdapter>>(
                () => new List<IPerformanceCounterAdapter>().ToArray());

            Name = name;

            CategoryType = categoryType;
            AdapterTypes = new[] {adapterType}.Concat(otherAdapterTypes);

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        internal PerformanceMeasurementDescriptor(IPerformanceMeasurementDescriptor other)
        {
            _adapterTypes = other.AdapterTypes;

            _lazyAdapters = new Lazy<IEnumerable<IPerformanceCounterAdapter>>(CreateAdapters);

            _categoryType = other.CategoryType;
            CategoryDescriptor = other.CategoryDescriptor;

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

        private IEnumerable<IPerformanceCounterAdapter> CreateAdapters()
        {
            const BindingFlags publicNonPublicInstance
                = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var ctors = AdapterTypes.Select(a => a.GetConstructor(publicNonPublicInstance, Type.DefaultBinder
                , new[] {typeof(IPerformanceMeasurementDescriptor)}, null)).ToArray();

            if (!ctors.Any())
                throw new InvalidOperationException("Invalid descriptor instance.");

            return ctors.Select(ctor => ctor.Invoke(new object[] {this})).Cast<IPerformanceCounterAdapter>();
        }

        public virtual IPerformanceMeasurementContext CreateContext()
        {
            return new PerformanceMeasurementContext(this, Adapters.ToArray());
        }

        private IEnumerable<CounterCreationData> GetCounterCreationData()
        {
            var prefix = Name;

            foreach (var datum in Adapters.SelectMany(a => a.Descriptor.CreationDataDescriptors))
            {
                var counterName = string.Join(".", prefix, datum.Name);
                yield return new CounterCreationData(counterName, datum.Help, datum.CounterType);
            }
        }

        private readonly CounterCreationDataComparer _dataComparer = new CounterCreationDataComparer();

        public virtual IEnumerable<CounterCreationData> Data
        {
            get { return GetCounterCreationData().OrderBy(x => x, _dataComparer); }
        }

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static bool Equals(IPerformanceMeasurementDescriptor a, IPerformanceMeasurementDescriptor b)
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

        public bool Equals(IPerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }

        public bool Equals(PerformanceMeasurementDescriptor other)
        {
            return Equals(this, other);
        }
    }
}
