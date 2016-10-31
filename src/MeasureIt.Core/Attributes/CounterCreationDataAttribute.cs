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
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get { return Descriptor.Name; }
            set { Descriptor.Name = value; }
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
        /// <param name="name"></param>
        public CounterCreationDataAttribute(string name)
        {
            _lazyDescriptor = new Lazy<ICounterCreationDataDescriptor>(
                () => new CounterCreationDataDescriptor {Name = name});
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
