using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Measurement
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public class MeasurementContext : Disposable, IMeasurementContext
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Random _rnd;

        // ReSharper disable once NotAccessedField.Local
        private readonly IInstrumentationDiscoveryOptions _options;

        public IEnumerable<IPerformanceCounterContext> CounterContexts { get; private set; }

        public IPerformanceCounterDescriptor Descriptor { get; private set; }

        internal MeasurementContext(IInstrumentationDiscoveryOptions options,
            IEnumerable<IPerformanceCounterContext> counterContexts)
        {
            // TODO: TBD: find the Descriptor from where?
            Descriptor = null;

            _rnd = options.RandomSeed.HasValue
                ? new Random(options.RandomSeed.Value)
                : new Random();

            _options = options;
            CounterContexts = counterContexts;
        }

        private class Gauge : Disposable
        {
            private readonly Stopwatch _stopwatch;

            private readonly IEnumerable<IPerformanceCounterContext> _contexts;

            internal Gauge(IEnumerable<IPerformanceCounterContext> contexts)
            {
                _stopwatch = new Stopwatch();
                _contexts = contexts;
            }

            internal void Start()
            {
                foreach (var context in _contexts)
                    context.Adapter.BeginMeasurement(context.Descriptor);

                _stopwatch.Start();
            }

            protected override void Dispose(bool disposing)
            {
                if (!IsDisposed && disposing)
                {
                    var elapsed = _stopwatch.Elapsed;

                    foreach (var context in _contexts)
                        context.Adapter.EndMeasurement(elapsed, context.Descriptor);
                }

                base.Dispose(disposing);
            }
        }

        public void Measure(Action aspect)
        {
            // TODO: TBD: not clear I would need anything else, descriptors, etc...
            using (var gauge = new Gauge(CounterContexts))
            {
                gauge.Disposed += (sender, e) =>
                {
                };

                // Do not actually start running until after we have setup.
                gauge.Start();

                aspect();
            }
        }

        public Task MeasureAsync(Func<Task> aspectGetter)
        {
            throw new NotImplementedException();
        }

        // TODO: TBD: how much of an active context needs to be disposed here...

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                // Fine to Dispose the Contexts themselves, but avoid disposing the contexts.Adapters.
                foreach (var context in CounterContexts)
                    context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
