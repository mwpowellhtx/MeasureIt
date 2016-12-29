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

        // TODO: TBD: is having a reference to HttpResponseMessage a good thing here? may want to be careful of that, especially with .NET Core work going on...

        /// <summary>
        /// Starts a new Measurement with optional <paramref name="startingCallback"/>.
        /// </summary>
        /// <param name="startingCallback">Callback invoked just prior to actually starting to gauge the measurement.</param>
        public virtual void Start(Action startingCallback = null)
        {
            startingCallback = startingCallback ?? delegate { };
            _gauge = new Gauge(Contexts);
            startingCallback();
            _gauge.Start();
        }

        /// <summary>
        /// Stops the Context from being evaluated.
        /// </summary>
        /// <returns></returns>
        public virtual ITwoStageMeasurementContext Stop()
        {
            _gauge.Stop();
            return this;
        }

        /// <summary>
        /// Sets the Error to the <paramref name="ex"/>.
        /// </summary>
        /// <param name="ex"></param>
        public virtual void SetError(Exception ex)
        {
            Descriptor.SetError(ex);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
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
