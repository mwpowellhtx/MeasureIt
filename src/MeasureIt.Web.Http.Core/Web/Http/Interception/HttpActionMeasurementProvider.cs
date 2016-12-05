using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Web.Http.Interception
{
    using Contexts;
    using Discovery;

    public class HttpActionMeasurementProvider : MeasurementProviderBase<
        ITwoStageMeasurementContext>, ITwoStageMeasurementProvider
    {
        private readonly Lazy<IHttpActionInstrumentationDiscoveryService> _lazyDiscoveryService;

        /// <summary>
        /// Gets the DiscoveryService.
        /// </summary>
        protected IHttpActionInstrumentationDiscoveryService DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

        public HttpActionMeasurementProvider(IInstrumentationDiscoveryOptions options
            , IHttpActionInstrumentationDiscoveryService discoveryService)
            : base(options)
        {
            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<IHttpActionInstrumentationDiscoveryService>(
                () =>
                {
                    discoveryService.Discover();
                    return discoveryService;
                }, execAndPubThreadSafety);
        }

        public override ITwoStageMeasurementContext GetMeasurementContext(Type targetType, MethodInfo method)
        {
            var descriptors = DiscoveryService.Measurements
                .Where(
                    d => d.RootType.IsRelatedTo(targetType)
                         && d.Method.GetBaseDefinition() == method.GetBaseDefinition()).ToArray();

            var descriptor = descriptors.SingleOrDefault();

            var o = Options;

            return descriptor != null
                ? new TwoStageMeasurementContext(o, descriptor, descriptor.CreateContext())
                : null;
        }
    }
}
