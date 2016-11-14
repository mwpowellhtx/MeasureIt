using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor
        : DescriptorBase
            , ICounterCreationDataDescriptor
    {
        public IPerformanceCounterAdapter Adapter { get; set; }

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
            : this((string) null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public CounterCreationDataDescriptor(string name)
        {
            Name = name;
            Help = string.Empty;
        }

        private CounterCreationDataDescriptor(CounterCreationDataDescriptor other)
            : base(other)
        {
            Copy(other);
        }

        private void Copy(CounterCreationDataDescriptor other)
        {
            // Moniker not the Name, per se.
            _nameMoniker = other._nameMoniker;
            Help = other.Help;
            CounterType = other.CounterType;
            Adapter = other.Adapter;
        }

        public CounterCreationData GetCounterCreationData()
        {
            // TODO: TBD: may look at using a moniker that builds the path of the name...
            return new CounterCreationData(string.Join(".", Adapter.Name, Name), Help, CounterType);
        }

        public override object Clone()
        {
            return new CounterCreationDataDescriptor(this);
        }
    }
}
