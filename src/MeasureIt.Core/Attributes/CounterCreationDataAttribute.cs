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
        private readonly ICounterCreationDataDescriptor _descriptor;

        public ICounterCreationDataDescriptor Descriptor
        {
            get { return _descriptor; }
        }

        /// <summary>
        /// Gets or sets the Suffix used in formulating the
        /// <see cref="CounterCreationData.CounterName"/>.
        /// </summary>
        public string Suffix
        {
            get { return Descriptor.Suffix; }
            set { Descriptor.Suffix = value; }
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
        /// Gets or sets the CounterType.
        /// </summary>
        public PerformanceCounterType CounterType
        {
            get { return Descriptor.CounterType; }
            set { Descriptor.CounterType = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CounterCreationDataAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="suffix"></param>
        public CounterCreationDataAttribute(string suffix)
        {
            _descriptor = new CounterCreationDataDescriptor {Suffix = suffix};
        }
    }
}
