namespace MeasureIt.Castle.Interception.Measurement
{
    using Contexts;
    using Discovery;

    public class InterceptionMeasurementProvider : MeasurementProviderBase<
        IRuntimeInstrumentationDiscoveryService>, IInterceptionMeasurementProvider
    {
        public InterceptionMeasurementProvider(IInstrumentationDiscoveryOptions options
            , IRuntimeInstrumentationDiscoveryService discoveryService)
            : base(options, discoveryService)
        {
        }
    }
}
