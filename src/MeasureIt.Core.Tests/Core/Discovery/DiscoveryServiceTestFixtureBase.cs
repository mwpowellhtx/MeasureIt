using System;
using System.Collections.Generic;
using System.Threading;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class DiscoveryServiceTestFixtureBase<TService> : IntegrationTestFixtureBase
        where TService : class, IInstrumentationDiscoveryService
    {
        protected abstract IInstrumentationDiscoveryOptions Options { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected delegate TService ServiceFactoryDelegate(IInstrumentationDiscoveryOptions options);

        protected abstract ServiceFactoryDelegate ServiceFactory { get; }

        protected virtual void OnBeforeDiscovery(TService service)
        {
            Assert.True(service.IsPending);
        }

        protected virtual void OnAfterDiscovery(TService service)
        {
            Assert.False(service.IsPending);
        }

        private readonly Lazy<TService> _lazyDiscoveryService;

        protected DiscoveryServiceTestFixtureBase()
        {
            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;
            _lazyDiscoveryService = new Lazy<TService>(() =>
                ServiceFactory(Options).VerifyDiscoveryService(OnBeforeDiscovery)
                    .VerifyDiscover().VerifyDiscoveryService(OnAfterDiscovery), execAndPubThreadSafety
                );
        }

        protected TService DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

        [Fact]
        public void CanDiscover()
        {
            // This is it. Just handshake that the Service Is no longer Pending.
            Assert.False(DiscoveryService.IsPending);
        }

        protected abstract void VerifyDiscoveredCounterAdapters(
            IEnumerable<IPerformanceCounterAdapter> discoveredItems);

        protected virtual void OnVerifyDescriptors(TService service)
        {
        }

        [Fact]
        public void VerifyDescriptors()
        {
            OnVerifyDescriptors(DiscoveryService);
        }
    }
}
