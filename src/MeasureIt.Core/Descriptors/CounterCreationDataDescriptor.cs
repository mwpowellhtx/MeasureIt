using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class CounterCreationDataDescriptor : ICounterCreationDataDescriptor
    {
        public IPerformanceCounterAdapterDescriptor AdapterDescriptor { get; set; }

        private Moniker _counterMoniker;

        public string CounterName
        {
            get { return _counterMoniker.Name; }
            set { _counterMoniker.Name = value; }
        }

        private Moniker _instanceMoniker;

        // TODO: TBD: may decide on another approach here?
        public string InstanceName
        {
            get { return _instanceMoniker.Name; }
            set { _instanceMoniker.Name = value; }
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
        /// <param name="counterName"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataDescriptor(string counterName, bool? readOnly = null)
            : this(counterName, null, readOnly)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="instanceName"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataDescriptor(string counterName, string instanceName, bool? readOnly = null)
        {
            _counterMoniker = new Moniker(counterName);
            _instanceMoniker = new Moniker(instanceName);
            Help = null;
            ReadOnly = readOnly;
            // Unlike InstanceLifetime, we cannot really know the CounterType at this moment.
            InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
        }

        public CounterCreationData GetCounterCreationData()
        {
            return new CounterCreationData(CounterName, Help, CounterType);
        }
    }
}
