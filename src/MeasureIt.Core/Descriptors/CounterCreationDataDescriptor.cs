using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor : ICounterCreationDataDescriptor
    {
        public IPerformanceCounterAdapterDescriptor AdapterDescriptor { get; set; }

        private IMoniker _nameMoniker;

        private IMoniker _instanceMoniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new NameMoniker(name);
        }

        public string Name
        {
            get { return _nameMoniker.ToString(); }
            set { _nameMoniker = GetNameMoniker(value) ?? DefaultMoniker.New(); }
        }

        public string InstanceName
        {
            get { return _instanceMoniker.ToString(); }
            set { _instanceMoniker = GetNameMoniker(value) ?? DefaultMoniker.New(); }
        }

        public string Help { get; set; }

        public bool? ReadOnly { get; set; }

        public PerformanceCounterType CounterType { get; set; }

        public PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CounterCreationDataDescriptor()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataDescriptor(string name, bool? readOnly = null)
            : this(name, null, readOnly)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="instanceName"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataDescriptor(string name, string instanceName, bool? readOnly = null)
        {
            Name = name;
            InstanceName = instanceName;
            Help = null;
            ReadOnly = readOnly;
            // Unlike InstanceLifetime, we cannot really know the CounterType at this moment.
            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
        }

        public CounterCreationData GetCounterCreationData()
        {
            // TODO: TBD: may need/want a different naming convention...
            return new CounterCreationData(Name, Help, CounterType);
        }
    }
}
