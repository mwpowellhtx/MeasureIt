using System;
using System.Diagnostics;
using System.Web.Http.Filters;

namespace MeasureIt.Web.Http.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class MeasurementFilterAttributeBase : ActionFilterAttribute, IMeasurePerformanceAttribute
    {
        public virtual IPerformanceMeasurementDescriptor Descriptor { get; private set; }

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
        /// Gets whether MayProceedUnabated.
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

        protected MeasurementFilterAttributeBase(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
        {
            Descriptor = new PerformanceMeasurementDescriptor(categoryType, adapterType, otherAdapterTypes);
        }
    }
}
