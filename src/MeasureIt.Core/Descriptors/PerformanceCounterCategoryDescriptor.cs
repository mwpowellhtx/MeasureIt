using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterCategoryDescriptor : IPerformanceCounterCategoryDescriptor
    {
        public Type Type { get; set; }

        private Moniker _moniker;

        public string Name
        {
            get { return _moniker.Name; }
            set { _moniker.Name = value; }
        }

        private string _help;

        public string Help
        {
            get { return _help; }
            set { _help = value ?? string.Empty; }
        }

        public PerformanceCounterCategoryType CategoryType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PerformanceCounterCategoryDescriptor()
            : this(string.Empty)
        {
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
        /// <param name="name"></param>
        /// <param name="help"></param>
        public PerformanceCounterCategoryDescriptor(string name, string help = null)
        {
            Name = name;
            Help = help;
            CategoryType = PerformanceCounterCategoryType.MultiInstance;
            CreationDataDescriptors = null;
        }

        public virtual CounterCreationDataCollection GetCounterCreationDataCollection()
        {
            var data = CreationDataDescriptors.Select(x => x.GetCounterCreationData());
            return new CounterCreationDataCollection(data.ToArray());
        }
    }
}
