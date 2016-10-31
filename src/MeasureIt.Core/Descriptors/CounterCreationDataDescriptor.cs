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

        public PerformanceCounterType CounterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CounterCreationDataDescriptor()
            : this(null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public CounterCreationDataDescriptor(string name)
        {
            Name = name;
            Help = null;
        }

        //public CounterCreationData GetCounterCreationData(IMeasurePerformanceDescriptor descriptor)
        //{
        //    // TODO: TBD: may need/want a different naming convention...
        //    return new CounterCreationData(string.Join(".", descriptor.CounterName, Name), Help, CounterType);
        //}
    }
}
