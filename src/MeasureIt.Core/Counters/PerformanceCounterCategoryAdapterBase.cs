using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    using Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public abstract class PerformanceCounterCategoryAdapterBase : IPerformanceCounterCategoryAdapter
    {
        private IMoniker _moniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new NameMoniker(name);
        }

        private static IMoniker GetTypeMoniker(Type type)
        {
            return type == null ? null : new TypeMoniker(type);
        }

        public string Name
        {
            get { return _moniker.ToString(); }
            private set { _moniker = GetNameMoniker(value) ?? GetTypeMoniker(GetType()) ?? DefaultMoniker.New(); }
        }

        public string Help { get; private set; }

        public PerformanceCounterCategoryType CategoryType { get; private set; }

        private IList<IPerformanceMeasurementDescriptor> _measurements;

        public virtual IList<IPerformanceMeasurementDescriptor> Measurements
        {
            get { return _measurements; }
            set
            {
                _measurements = (value ?? new List<IPerformanceMeasurementDescriptor>())
                    .Select(d =>
                    {
                        d.CategoryAdapter = this;
                        return d;
                    }).ToList();
            }
        }

        public virtual IList<IPerformanceMeasurementDescriptor> InternalMeasurements
        {
            get
            {
                return Measurements.ToBidirectionalList(
                    added => added.CategoryAdapter = this
                    , removed => removed.CategoryAdapter = null
                    );
            }
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="help"></param>
        /// <param name="categoryType"></param>
        protected PerformanceCounterCategoryAdapterBase(string name, string help = null,
            PerformanceCounterCategoryType categoryType = PerformanceCounterCategoryType.MultiInstance)
        {
            Initialize(name, help, categoryType);
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="help"></param>
        /// <param name="categoryType"></param>
        protected PerformanceCounterCategoryAdapterBase(string help = null,
            PerformanceCounterCategoryType categoryType = PerformanceCounterCategoryType.MultiInstance)
            : this(null, help, categoryType)
        {
        }

        private void Initialize(string name, string help = null,
            PerformanceCounterCategoryType categoryType = PerformanceCounterCategoryType.MultiInstance)
        {
            Name = name;
            Help = help;
            CategoryType = categoryType;
            Measurements = null;
        }

        public IReadOnlyCollection<ICounterCreationDataDescriptor> CreationData
        {
            get
            {
                var adapters = Measurements.SelectMany(d => d.Adapters).ToArray();
                var descriptors = adapters.SelectMany(a => a.CreationData).ToList();
                return new ReadOnlyCollection<ICounterCreationDataDescriptor>(descriptors);
            }
        }
    }
}
