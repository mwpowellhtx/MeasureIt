using System;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterAdapterAttribute : Attribute, IPerformanceCounterAdapterAttribute
    {
        private readonly Lazy<IPerformanceCounterAdapterDescriptor> _lazyDescriptor;

        public IPerformanceCounterAdapterDescriptor Descriptor
        {
            get { return _lazyDescriptor.Value; }
        }

        public string Name
        {
            get { return Descriptor.CounterName; }
            set { Descriptor.CounterName = value; }
        }

        public string Help
        {
            get { return Descriptor.CounterHelp; }
            set { Descriptor.CounterHelp = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PerformanceCounterAdapterAttribute()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="counterName"></param>
        public PerformanceCounterAdapterAttribute(string counterName)
        {
            _lazyDescriptor = new Lazy<IPerformanceCounterAdapterDescriptor>(
                () => new PerformanceCounterAdapterDescriptor(counterName));
        }
    }
}
