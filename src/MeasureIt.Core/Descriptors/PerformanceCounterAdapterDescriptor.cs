using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterAdapterDescriptor : DescriptorBase, IPerformanceCounterAdapterDescriptor
    {
        private IMoniker _moniker;

        private static IMoniker GetNameMoniker(string name)
        {
            return new NameMoniker(name);
        }

        public string CounterName
        {
            get { return _moniker.ToString(); }
            set { _moniker = GetNameMoniker(value) ?? DefaultMoniker.New(); }
        }

        public string CounterHelp { get; set; }

        private Type _adapterType;

        /// <summary>
        /// Gets or sets the AdapterType.
        /// </summary>
        public Type AdapterType
        {
            get { return _adapterType; }
            set
            {
                _adapterType = value.VerifyType<IPerformanceCounterAdapter>();

                CreationDataDescriptors = _adapterType.GetAttributeValues(
                    (CounterCreationDataAttribute a) => a.Descriptor)
                    .OrderBy(d => d.CounterType);
            }
        }

        private IEnumerable<ICounterCreationDataDescriptor> _dataDescriptors;

        /// <summary>
        /// Gets the CreationDataDescriptors.
        /// </summary>
        public IEnumerable<ICounterCreationDataDescriptor> CreationDataDescriptors
        {
            get { return _dataDescriptors; }
            set { _dataDescriptors = (value ?? new List<ICounterCreationDataDescriptor>()).ToArray(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        public PerformanceCounterAdapterDescriptor(string counterName)
            : this(counterName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="counterHelp"></param>
        public PerformanceCounterAdapterDescriptor(string counterName, string counterHelp)
        {
            CounterName = counterName;
            CounterHelp = counterHelp ?? string.Empty;
        }
    }
}
