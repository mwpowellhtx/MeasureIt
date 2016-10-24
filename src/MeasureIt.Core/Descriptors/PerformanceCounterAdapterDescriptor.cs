using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterAdapterDescriptor : IPerformanceCounterAdapterDescriptor
    {
        private Moniker _counterMoniker;

        public string CounterName
        {
            get { return _counterMoniker.Name; }
            set { _counterMoniker.Name = value; }
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
                _adapterType = value;
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
            _counterMoniker = new Moniker(counterName);
            CounterHelp = counterHelp ?? string.Empty;
        }
    }
}
