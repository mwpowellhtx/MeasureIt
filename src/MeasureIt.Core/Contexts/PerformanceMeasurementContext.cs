using System;
using System.Collections.Generic;

namespace MeasureIt.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceMeasurementContext : Disposable, IPerformanceMeasurementContext
    {
        private readonly IPerformanceMeasurementDescriptor _descriptor;

        private readonly IEnumerable<IPerformanceCounterAdapter> _adapters;

        private IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _adapters; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="adapters"></param>
        internal PerformanceMeasurementContext(IPerformanceMeasurementDescriptor descriptor,
            params IPerformanceCounterAdapter[] adapters)
        {
            _descriptor = descriptor;
            _adapters = adapters;
        }

        public void BeginMeasurement()
        {
            Adapters.IfNotNull(a => a.BeginMeasurement(_descriptor));
        }

        public void EndMeasurement(TimeSpan elapsed)
        {
            Adapters.IfNotNull(a => a.EndMeasurement(elapsed, _descriptor));
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Adapters.IfNotNull(a => a.Dispose());
            }

            base.Dispose(disposing);
        }
    }
}
