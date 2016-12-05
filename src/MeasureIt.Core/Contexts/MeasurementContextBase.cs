using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// Establishes a MeasurementContext base class.
    /// </summary>
    public abstract class MeasurementContextBase : ContextBase
    {
        /// <summary>
        /// Gets the Rnd random number generator.
        /// </summary>
        protected Random Rnd { get; private set; }

        /// <summary>
        /// Gets the Contexts.
        /// </summary>
        protected IEnumerable<IPerformanceMeasurementContext> Contexts { get; private set; }

        /// <summary>
        /// Gets the Descriptor.
        /// </summary>
        public IPerformanceMeasurementDescriptor Descriptor { get; private set; }

        /// <summary>
        ///  Gets the Options.
        ///  </summary>
        protected IInstrumentationDiscoveryOptions Options { get; private set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="options"></param>
        /// <param name="descriptor"></param>
        /// <param name="contexts"></param>
        protected MeasurementContextBase(IInstrumentationDiscoveryOptions options,
            IPerformanceMeasurementDescriptor descriptor,
            params IPerformanceMeasurementContext[] contexts)
        {
            Options = options;

            Descriptor = descriptor;

            Rnd = options.RandomSeed.HasValue
                ? new Random(options.RandomSeed.Value)
                : new Random();

            Contexts = contexts;
        }

        /// <summary>
        /// Provides a simple Gauge for measuring the time of the Contexts.
        /// </summary>
        protected class Gauge : Disposable
        {
            private readonly Stopwatch _stopwatch;

            private readonly IEnumerable<IPerformanceMeasurementContext> _contexts;

            internal Gauge(IEnumerable<IPerformanceMeasurementContext> contexts)
            {
                _stopwatch = new Stopwatch();
                _contexts = contexts;
            }

            internal void Start()
            {
                foreach (var context in _contexts)
                    context.BeginMeasurement();

                if (_stopwatch.IsRunning)
                    _stopwatch.Restart();
                else
                    _stopwatch.Start();
            }

            internal void Stop()
            {
                _stopwatch.Stop();
            }

            protected override void Dispose(bool disposing)
            {
                if (!IsDisposed && disposing)
                {
                    var elapsed = _stopwatch.Elapsed;

                    foreach (var context in _contexts)
                    {
                        context.EndMeasurement(elapsed);
                        context.Dispose();
                    }
                }

                base.Dispose(disposing);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                // Fine to Dispose the Contexts themselves, but avoid disposing the contexts.Adapters.
                foreach (var context in Contexts)
                    context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
