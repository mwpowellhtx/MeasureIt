using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public class MeasurementContext : ContextBase, IMeasurementContext
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Random _rnd;

        // ReSharper disable once NotAccessedField.Local
        private readonly IInstrumentationDiscoveryOptions _options;

        private readonly IEnumerable<IPerformanceMeasurementContext> _contexts;

        public IPerformanceMeasurementDescriptor Descriptor { get; private set; }

        internal MeasurementContext(IInstrumentationDiscoveryOptions options,
            IEnumerable<IPerformanceMeasurementContext> contexts)
        {
            // TODO: TBD: find the Descriptor from where?
            Descriptor = null;

            _rnd = options.RandomSeed.HasValue
                ? new Random(options.RandomSeed.Value)
                : new Random();

            _options = options;
            _contexts = contexts;
        }

        private class Gauge : Disposable
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

        public void Measure(Action aspect)
        {
            // TODO: TBD: not clear I would need anything else, descriptors, etc...
            using (var gauge = new Gauge(_contexts))
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
            return Task.Run(() =>
            {
                using (var gauge = new Gauge(_contexts))
                {
                    gauge.Disposed += (sender, e) =>
                    {
                    };

                    // Start running after setup.
                    gauge.Start();

                    var aspect = aspectGetter();

                    aspect.Wait();
                }
            });
        }

        // TODO: TBD: how much of an active context needs to be disposed here...

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                // Fine to Dispose the Contexts themselves, but avoid disposing the contexts.Adapters.
                foreach (var context in _contexts)
                    context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
