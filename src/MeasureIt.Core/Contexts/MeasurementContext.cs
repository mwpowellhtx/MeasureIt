using System;
using System.Threading.Tasks;

namespace MeasureIt.Contexts
{
    using Discovery;

    // TODO: TBD: could potentially re-factor this into the Castle.Interception assembly...
    /// <summary>
    /// 
    /// </summary>
    public class MeasurementContext : MeasurementContextBase, IInterceptionMeasurementContext
    {
        internal MeasurementContext(IInstrumentationDiscoveryOptions options,
            IPerformanceMeasurementDescriptor descriptor,
            params IPerformanceMeasurementContext[] contexts)
            : base(options, descriptor, contexts)
        {
        }

        public void Measure(Action aspect)
        {
            // Reset the Error condition prior to any timed gauges.
            Descriptor.SetError();

            // TODO: TBD: not clear I would need anything else, descriptors, etc...
            using (var gauge = new Gauge(Contexts))
            {
                // Do not actually start running until after we have setup.
                gauge.Start();

                try
                {
                    // Try the aspect, providing an opportunity to measure the Error rate.
                    aspect();
                }
                catch (Exception ex)
                {
                    Descriptor.SetError(ex);
                    // At this level we throw.
                    throw;
                }
            }
        }

        public Task MeasureAsync(Func<Task> aspectGetter)
        {
            return Task.Run(() =>
            {
                // Reset the Error condition.
                Descriptor.SetError();

                using (var gauge = new Gauge(Contexts))
                {
                    // Start running after setup.
                    gauge.Start();

                    try
                    {
                        // Ditto the non-threaded Measure method.
                        var aspect = aspectGetter();

                        aspect.Wait();
                    }
                    catch (Exception ex)
                    {
                        Descriptor.SetError(ex);
                        // Also ditto the non-threaded Measure method.
                        throw;
                    }
                }
            });
        }
    }
}
