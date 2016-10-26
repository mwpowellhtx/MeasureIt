using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MeasureIt.Measurement;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterDescriptor
        : IPerformanceCounterDescriptor
            , IEquatable<PerformanceCounterDescriptor>
    {
        // TODO: TBD: what, if anything, to do about the RandomSeed?
        private Moniker _counterMoniker;

        public string CounterName
        {
            get { return _counterMoniker.Name; }
            set { _counterMoniker.Name = value; }
        }

        private Type _categoryType;

        public Type CategoryType
        {
            get { return _categoryType; }
            set
            {
                _categoryType = value;
                CategoryDescriptor = value.GetAttributeValue((PerformanceCounterCategoryAttribute a) => a.Descriptor);
            }
        }

        public IPerformanceCounterCategoryDescriptor CategoryDescriptor { get; private set; }

        private Type _adapterType;

        /// <summary>
        /// 
        /// </summary>
        public Type AdapterType
        {
            get { return _adapterType; }
            set
            {
                _adapterType = value.VerifyAdapterType();
                _lazyAdapterDescriptor = new Lazy<IPerformanceCounterAdapterDescriptor>(
                    () => value.GetAttributeValue((PerformanceCounterAdapterAttribute a) => a.Descriptor));
            }
        }

        private Lazy<IPerformanceCounterAdapterDescriptor> _lazyAdapterDescriptor;

        public IPerformanceCounterAdapterDescriptor AdapterDescriptor
        {
            get { return _lazyAdapterDescriptor.Value; }
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

        public MethodInfo Method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adapterType"></param>
        /// <param name="categoryType"></param>
        public PerformanceCounterDescriptor(Type adapterType, Type categoryType)
            : this(string.Empty, adapterType, categoryType)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="adapterType"></param>
        /// <param name="categoryType"></param>
        public PerformanceCounterDescriptor(string counterName, Type adapterType, Type categoryType)
        {
            _counterMoniker = new Moniker(counterName ?? string.Empty);

            AdapterType = adapterType;
            CategoryType = categoryType;

            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;

            PublishCounters = true;
            PublishEvent = true;
            ThrowPublishErrors = false;

            SampleRate = Constants.DefaultSampleRate;
        }

        internal PerformanceCounterDescriptor(IPerformanceCounterDescriptor other)
        {
            _adapterType = other.AdapterType;

            _lazyAdapterDescriptor = new Lazy<IPerformanceCounterAdapterDescriptor>(() => other.AdapterDescriptor);

            if (AdapterDescriptor == null)
                throw new ArgumentException("Invalid descriptor instance.", "other");

            _categoryType = other.CategoryType;
            CategoryDescriptor = other.CategoryDescriptor;

            _counterMoniker.Name = other.CounterName;
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

            return AdapterDescriptor.CreationDataDescriptors.Select(x =>
            {
                var readOnly = x.ReadOnly;
                var counterName = x.CounterName;
                // TODO: TBD: may want to account for non-instance name here...
                var instanceName = x.InstanceName;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (readOnly)
                {
                    case true:
                    case false:
                        return new PerformanceCounter(categoryName, counterName, instanceName, readOnly.Value);
                    default:
                        return new PerformanceCounter(categoryName, counterName, instanceName);
                }
            });
        }

        public virtual IPerformanceCounterAdapter CreateAdapter()
        {
            const BindingFlags nonPublicInstance = BindingFlags.NonPublic | BindingFlags.Instance;

            var ctor = AdapterType.GetConstructor(nonPublicInstance, Type.DefaultBinder,
                new[] {typeof(IPerformanceCounterDescriptor)}, null);

            return (IPerformanceCounterAdapter) ctor.Invoke(new object[] {this});
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
                            || a.AdapterType == null || b.AdapterType == null)
                       && a.CategoryType == b.CategoryType
                       && a.AdapterType == b.AdapterType
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
