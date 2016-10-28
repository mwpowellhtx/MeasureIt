using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Measurement;

    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterDescriptor
        : IPerformanceCounterDescriptor
            , IEquatable<PerformanceCounterDescriptor>
    {
        // TODO: TBD: what, if anything, to do about the RandomSeed?
        private IMoniker _counterMoniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new NameMoniker(name);
        }

        private static IMoniker GetMethodMoniker(MethodInfo method)
        {
            return method == null ? null : new MethodInfoMoniker(method);
        }

        public string CounterName
        {
            get { return _counterMoniker.ToString(); }
            set { _counterMoniker = GetNameMoniker(value) ?? GetMethodMoniker(Method) ?? DefaultMoniker.New(); }
        }

        private Type _categoryType;

        public Type CategoryType
        {
            get { return _categoryType; }
            set
            {
                _categoryType = value.VerifyType<IPerformanceCounterCategoryAdapter>();
                // TODO: TBD: not only identify the descriptor here, but also register itself with it?
                CategoryDescriptor = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);
            }
        }

        public IPerformanceCounterCategoryDescriptor CategoryDescriptor { get; private set; }

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

                _lazyAdapterDescriptors = new Lazy<IEnumerable<IPerformanceCounterAdapterDescriptor>>(
                    () => (value ?? new Type[0]).Select(t => t.GetAttributeValue(
                        (PerformanceCounterAdapterAttribute a) => a.Descriptor)).ToArray());
            }
        }

        private Lazy<IEnumerable<IPerformanceCounterAdapterDescriptor>> _lazyAdapterDescriptors;

        public IEnumerable<IPerformanceCounterAdapterDescriptor> AdapterDescriptors
        {
            get { return _lazyAdapterDescriptors.Value; }
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
                CounterName = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceCounterDescriptor(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : this(null, categoryType, adapterType, otherAdapterTypes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceCounterDescriptor(string counterName, Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
        {
            CounterName = counterName;

            AdapterTypes = new[] {adapterType}.Concat(otherAdapterTypes);
            CategoryType = categoryType;

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        internal PerformanceCounterDescriptor(IPerformanceCounterDescriptor other)
        {
            _adapterTypes = other.AdapterTypes;

            _lazyAdapterDescriptors = new Lazy<IEnumerable<IPerformanceCounterAdapterDescriptor>>(
                () => other.AdapterDescriptors);

            if (!AdapterDescriptors.Any())
                throw new ArgumentException("Invalid descriptor instance.", "other");

            _categoryType = other.CategoryType;
            CategoryDescriptor = other.CategoryDescriptor;

            CounterName = other.CounterName;
            InstanceLifetime = other.InstanceLifetime;

            RootType = other.RootType;
            Method = other.Method;

            PublishCounters = other.PublishCounters;
            PublishEvent = other.PublishEvent;
            ThrowPublishErrors = other.ThrowPublishErrors;

            SampleRate = other.SampleRate;

            ReadOnly = other.ReadOnly;
        }

        public IEnumerable<PerformanceCounter> GetPerformanceCounters()
        {
            var categoryName = CategoryDescriptor.Name;

            return AdapterDescriptors.SelectMany(d => d.CreationDataDescriptors.Select(x =>
            {
                var readOnly = x.ReadOnly;
                // TODO: TBD: build a Name/path here...
                var name = x.Name;
                // TODO: TBD: may want to account for non-instance name here...
                var instanceName = x.InstanceName;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (readOnly)
                {
                    case true:
                    case false:
                        return new PerformanceCounter(categoryName, name, instanceName, readOnly.Value);
                    default:
                        return new PerformanceCounter(categoryName, name, instanceName);
                }
            }));
        }

        public virtual IEnumerable<IPerformanceCounterAdapter> CreateAdapters()
        {
            const BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

            var ctors = AdapterTypes.Select(a => a.GetConstructor(nonPublicInstance, Type.DefaultBinder,
                new[] {typeof(IPerformanceCounterDescriptor)}, null));

            return ctors.Select(ctor => ctor.Invoke(new object[] {this})).Cast<IPerformanceCounterAdapter>();
        }

        public virtual IPerformanceCounterContext CreateContext()
        {
            return new PerformanceCounterContext(this);
        }

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static bool Equals(IPerformanceCounterDescriptor a, IPerformanceCounterDescriptor b)
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
                       && a.AdapterTypes.SequenceEqual(b.AdapterTypes)
                       );
        }

        public bool Equals(IPerformanceCounterDescriptor other)
        {
            return Equals(this, other);
        }

        public bool Equals(PerformanceCounterDescriptor other)
        {
            return Equals(this, other);
        }
    }
}
