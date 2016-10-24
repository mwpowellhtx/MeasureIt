using System;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents a <see cref="Type"/> rooted <see cref="PerformanceCounter"/>
    /// <see cref="Attribute"/>. The benefit of being rooted to a Type is that we can verify at
    /// compile time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CounterCreationDataAttribute : Attribute, ICounterCreationDataAttribute
    {
        private readonly Lazy<ICounterCreationDataDescriptor> _lazyDescriptor;

        public ICounterCreationDataDescriptor Descriptor
        {
            get { return _lazyDescriptor.Value; }
        }

        /// <summary>
        /// Gets or sets the CounterName.
        /// </summary>
        public string CounterName
        {
            get { return Descriptor.CounterName; }
            set { Descriptor.CounterName = value; }
        }

        /// <summary>
        /// Gets or sets the Help.
        /// </summary>
        public string Help
        {
            get { return Descriptor.Help; }
            set { Descriptor.Help = value; }
        }

        /// <summary>
        /// Gets or sets whether ReadOnly. Leaving unspecified assumes read-only.
        /// </summary>
        public bool? ReadOnly
        {
            get { return Descriptor.ReadOnly; }
            set { Descriptor.ReadOnly = value; }
        }

        /// <summary>
        /// Gets or sets the CounterType.
        /// </summary>
        public PerformanceCounterType CounterType
        {
            get { return Descriptor.CounterType; }
            set { Descriptor.CounterType = value; }
        }

        /// <summary>
        /// Gets or sets the InstanceLifetime.
        /// </summary>
        public PerformanceCounterInstanceLifetime InstanceLifetime
        {
            get { return Descriptor.InstanceLifetime; }
            set { Descriptor.InstanceLifetime = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CounterCreationDataAttribute()
            : this(string.Empty, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterName"></param>
        public CounterCreationDataAttribute(string counterName)
            : this(counterName, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readOnly"></param>
        public CounterCreationDataAttribute(bool readOnly)
            : this(string.Empty, (bool?) readOnly)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="readOnly"></param>
        public CounterCreationDataAttribute(string counterName, bool readOnly)
            : this(counterName, (bool?) readOnly)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="readOnly"></param>
        private CounterCreationDataAttribute(string counterName, bool? readOnly)
        {
            _lazyDescriptor = new Lazy<ICounterCreationDataDescriptor>(
                () => new CounterCreationDataDescriptor {CounterName = counterName, ReadOnly = readOnly});
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="attr"></param>
        ///// <returns></returns>
        //public static explicit operator CounterCreationData(CounterCreationDataAttribute attr)
        //{
        //    return attr.GetCounterCreationData();
        //}
    }
}
