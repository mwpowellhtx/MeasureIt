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
        private string _name;

        /// <summary>
        /// Nets or sets the adapter Name.
        /// </summary>
        public string Name
        {
            get { return _name ?? GetType().FullName; }
            set { _name = string.IsNullOrEmpty(value) ? null : value; }
        }

        /// <summary>
        /// Gets the Help.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// Gets the CategoryType.
        /// </summary>
        public PerformanceCounterCategoryType CategoryType { get; private set; }

        private IList<IPerformanceMeasurementDescriptor> _measurements;

        /// <summary>
        /// Gets or sets the Measurements.
        /// </summary>
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

        /// <summary>
        /// Gets the <see cref="Measurements"/> for internal use.
        /// </summary>
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

        /// <summary>
        /// Gets the CreationData corresponding with the Adapter.
        /// </summary>
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
