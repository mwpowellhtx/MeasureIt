using System;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Represents a <see cref="Type"/> rooted <see cref="PerformanceCounter"/>
    /// <see cref="Attribute"/>. The benefit of being rooted to a Type is that we can verify at
    /// compile time.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MeasurePerformanceAttribute : Attribute, IMeasurePerformanceAttribute
    {
        private readonly IPerformanceMeasurementDescriptor _descriptor;

        public IPerformanceMeasurementDescriptor Descriptor
        {
            get { return _descriptor; }
        }

        /// <summary>
        /// Gets or sets whether <see cref="PerformanceCounter.ReadOnly"/>. Leaving unspecified
        /// assumes read-only.
        /// </summary>
        public bool ReadOnly
        {
            get { return Descriptor.ReadOnly ?? false; }
            set { Descriptor.ReadOnly = value; }
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
        /// Gets or sets whether to PublishCounters.
        /// </summary>
        public bool PublishCounters
        {
            get { return Descriptor.PublishCounters; }
            set { Descriptor.PublishCounters = value; }
        }

        /// <summary>
        /// Gets or sets whether to ThrowPublishErrors.
        /// </summary>
        public bool ThrowPublishErrors
        {
            get { return Descriptor.ThrowPublishErrors; }
            set { Descriptor.ThrowPublishErrors = value; }
        }

        /// <summary>
        /// Gets or sets whether to PublishEvent.
        /// </summary>
        public bool PublishEvent
        {
            get { return Descriptor.PublishEvent; }
            set { Descriptor.PublishEvent = value; }
        }

        /// <summary>
        /// Gets whether MayproceedUnabated.
        /// </summary>
        public bool MayProceedUnabated
        {
            get { return Descriptor.ThrowPublishErrors; }
        }

        /// <summary>
        /// Gets or sets the SampleRate.
        /// </summary>
        public double SampleRate
        {
            get { return Descriptor.SampleRate; }
            set { Descriptor.SampleRate = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public MeasurePerformanceAttribute(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : this(string.Empty, categoryType, adapterType, otherAdapterTypes)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="counterName"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public MeasurePerformanceAttribute(string counterName, Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
        {
            _descriptor = new PerformanceMeasurementDescriptor(counterName, categoryType, adapterType, otherAdapterTypes);
        }
    }
}
