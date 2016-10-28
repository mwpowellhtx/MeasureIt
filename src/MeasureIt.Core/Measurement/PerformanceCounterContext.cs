using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Measurement
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterContext : Disposable, IPerformanceCounterContext
    {
        private readonly Lazy<IEnumerable<IPerformanceCounterAdapter>> _lazyAdapters;

        private IEnumerable<IPerformanceCounterAdapter> Adapters
        {
            get { return _lazyAdapters.Value; }
        }

        private readonly IPerformanceCounterDescriptor _descriptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        internal PerformanceCounterContext(IPerformanceCounterDescriptor descriptor)
        {
            _descriptor = descriptor;
            _lazyAdapters = new Lazy<IEnumerable<IPerformanceCounterAdapter>>(
                () => (_descriptor == null
                    ? new IPerformanceCounterAdapter[0]
                    : _descriptor.CreateAdapters()).ToList()
                );
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
