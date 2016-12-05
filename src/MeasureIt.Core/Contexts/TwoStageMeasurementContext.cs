using System;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// Provides a TwoStageMeasurementContext for purposes of supporting web based Action Filters.
    /// </summary>
    public class TwoStageMeasurementContext : MeasurementContextBase, ITwoStageMeasurementContext
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="descriptor"></param>
        /// <param name="contexts"></param>
        public TwoStageMeasurementContext(IInstrumentationDiscoveryOptions options,
            IPerformanceMeasurementDescriptor descriptor,
            params IPerformanceMeasurementContext[] contexts)
            : base(options, descriptor, contexts)
        {
            // Clear the Error condition in this case.
            Descriptor.SetError();
        }

        private Gauge _gauge;

        public virtual void Start()
        {
            _gauge = new Gauge(Contexts);
            _gauge.Start();
        }

        public virtual ITwoStageMeasurementContext Stop()
        {
            _gauge.Stop();
            return this;
        }

        public virtual void SetError(Exception ex)
        {
            Descriptor.SetError(ex);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                _gauge.Dispose();
                _gauge = null;
            }
            base.Dispose(disposing);
        }
    }
}
