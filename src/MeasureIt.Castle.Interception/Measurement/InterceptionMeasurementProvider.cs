namespace MeasureIt.Castle.Interception.Measurement
{
    using Discovery;
    using MeasureIt.Measurement;

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
