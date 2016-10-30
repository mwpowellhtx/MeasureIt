using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor : DescriptorBase, ICounterCreationDataDescriptor
    {
        public IPerformanceCounterAdapterDescriptor AdapterDescriptor { get; set; }

        private IMoniker _nameMoniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return string.IsNullOrEmpty(name) ? null : new NameMoniker(name);
        }

        public string Name
        {
            get { return _nameMoniker.ToString(); }
            set { _nameMoniker = GetNameMoniker(value) ?? DefaultMoniker.New(); }
        }

        public string Help { get; set; }

        public bool? ReadOnly { get; set; }

        public PerformanceCounterType CounterType { get; set; }

        public PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CounterCreationDataDescriptor()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataDescriptor(string name, bool? readOnly = null)
        {
            Name = name;
            Help = null;
            ReadOnly = readOnly;
            // Unlike InstanceLifetime, we cannot really know the CounterType at this moment.
            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
        }

        //public CounterCreationData GetCounterCreationData(IMeasurePerformanceDescriptor descriptor)
        //{
        //    // TODO: TBD: may need/want a different naming convention...
        //    return new CounterCreationData(string.Join(".", descriptor.CounterName, Name), Help, CounterType);
        //}
    }
}
