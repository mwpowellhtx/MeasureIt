using System;

namespace MeasureIt.Measurement
{
    /// <summary>
    /// 
    /// </summary>
    public class PerformanceCounterContext : Disposable, IPerformanceCounterContext
    {
        private readonly Lazy<IPerformanceCounterAdapter> _lazyAdapter;

        private IPerformanceCounterAdapter Adapter
        {
            get { return _lazyAdapter.Value; }
        }

        private readonly IPerformanceCounterDescriptor _descriptor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        internal PerformanceCounterContext(IPerformanceCounterDescriptor descriptor)
        {
            _descriptor = descriptor;
            _lazyAdapter = new Lazy<IPerformanceCounterAdapter>(
                () => _descriptor == null ? null : _descriptor.CreateAdapter());
        }

        public void BeginMeasurement()
        {
            Adapter.IfNotNull(a => a.BeginMeasurement(_descriptor));
        }

        public void EndMeasurement(TimeSpan elapsed)
        {
            Adapter.IfNotNull(a => a.EndMeasurement(elapsed, _descriptor));
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Adapter.IfNotNull(a => a.Dispose());
            }

            base.Dispose(disposing);
        }
    }
}
