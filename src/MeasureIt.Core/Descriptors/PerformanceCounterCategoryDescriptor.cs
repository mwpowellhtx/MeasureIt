using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterCategoryDescriptor : DescriptorBase, IPerformanceCounterCategoryDescriptor
    {
        public Type Type { get; set; }

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
            set { _moniker = GetNameMoniker(value) ?? GetTypeMoniker(Type) ?? DefaultMoniker.New(); }
        }

        private string _help;

        public string Help
        {
            get { return _help; }
            set { _help = value ?? string.Empty; }
        }

        public PerformanceCounterCategoryType CategoryType { get; set; }

        private Lazy<IList<IPerformanceMeasurementDescriptor>> _lazyMeasurements;

        public virtual IList<IPerformanceMeasurementDescriptor> Measurements
        {
            get { return _lazyMeasurements.Value; }
            private set
            {
                _lazyMeasurements = new Lazy<IList<IPerformanceMeasurementDescriptor>>(
                    () => (value ?? new List<IPerformanceMeasurementDescriptor>(0)).ToList());
            }
        }

        private IEnumerable<ICounterCreationDataDescriptor> _creationDataDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ICounterCreationDataDescriptor> CreationDataDescriptors
        {
            get { return _creationDataDescriptors; }
            set { _creationDataDescriptors = (value ?? new List<ICounterCreationDataDescriptor>()).ToArray(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public PerformanceCounterCategoryDescriptor()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="help"></param>
        public PerformanceCounterCategoryDescriptor(string name, string help = null)
        {
            Name = name;
            Help = help;
            CategoryType = PerformanceCounterCategoryType.MultiInstance;
            CreationDataDescriptors = null;
            Measurements = null;
        }

        public virtual PerformanceCounterCategory CreateCategory()
        {
            var items = Measurements.SelectMany(d => d.Data).ToArray();
            var data = new CounterCreationDataCollection(items);
            return PerformanceCounterCategory.Create(Name, Help, CategoryType, data);
        }

        /// <summary>
        /// Tries to Delete the Category.
        /// </summary>
        public virtual bool TryDeleteCategory()
        {
            var exists = PerformanceCounterCategory.Exists(Name);
            PerformanceCounterCategory.Delete(Name);
            return exists;
        }
    }
}
