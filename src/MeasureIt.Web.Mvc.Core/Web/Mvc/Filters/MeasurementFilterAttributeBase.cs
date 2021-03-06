using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace MeasureIt.Web.Mvc.Filters
{
    using Discovery;

    /// <summary>
    /// Measurement filter base class.
    /// </summary>
    public abstract class MeasurementFilterAttributeBase : ActionFilterAttribute, IMvcMeasurePerformanceAttribute
    {
        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        public virtual IMvcPerformanceMeasurementDescriptor Descriptor { get; }

        /// <summary>
        /// Sets whether <see cref="PerformanceCounter.ReadOnly"/>. Leaving unspecified assumes
        /// read-only.
        /// </summary>
        public bool ReadOnly
        {
            get { return Descriptor.ReadOnly ?? false; }
            set { Descriptor.ReadOnly = value; }
        }

        /// <summary>
        /// Sets the InstanceLifetime.
        /// </summary>
        public PerformanceCounterInstanceLifetime InstanceLifetime
        {
            get { return Descriptor.InstanceLifetime; }
            set { Descriptor.InstanceLifetime = value; }
        }

        /// <summary>
        /// Sets whether to PublishCounters.
        /// </summary>
        public bool PublishCounters
        {
            get { return Descriptor.PublishCounters; }
            set { Descriptor.PublishCounters = value; }
        }

        /// <summary>
        /// Sets whether to ThrowPublishErrors.
        /// </summary>
        public bool ThrowPublishErrors
        {
            get { return Descriptor.ThrowPublishErrors; }
            set { Descriptor.ThrowPublishErrors = value; }
        }

        /// <summary>
        /// Sets whether to PublishEvent.
        /// </summary>
        public bool PublishEvent
        {
            get { return Descriptor.PublishEvent; }
            set { Descriptor.PublishEvent = value; }
        }

        /// <summary>
        /// Sets the SampleRate.
        /// </summary>
        public double SampleRate
        {
            get { return Descriptor.SampleRate; }
            set { Descriptor.SampleRate = value; }
        }

        private IEnumerable<MeasurementBoundary> GetBoundaries()
        {
            yield return Descriptor.Boundary.Start;
            yield return Descriptor.Boundary.Stop;
        }

        /// <summary>
        /// Sets the StartBoundary <see cref="MeasurementBoundary"/>.
        /// </summary>
        public MeasurementBoundary[] Boundary
        {
            get { return GetBoundaries().ToArray(); }
            set
            {
                MeasurementBoundary start;
                MeasurementBoundary stop;
                value.VerifyBoundaries(out start, out stop);
                Descriptor.Boundary.Set(start, stop);
            }
        }

        /// <summary>
        /// Sets the Start <see cref="MeasurementBoundary"/>.
        /// </summary>
        public MeasurementBoundary StartBoundary => Descriptor.Boundary.Start;

        /// <summary>
        /// Sets the Stop <see cref="MeasurementBoundary"/>.
        /// </summary>
        public MeasurementBoundary StopBoundary => Descriptor.Boundary.Stop;

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        protected MeasurementFilterAttributeBase(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
        {
            Descriptor = new MvcPerformanceMeasurementDescriptor(categoryType, adapterType, otherAdapterTypes);
        }
    }
}
