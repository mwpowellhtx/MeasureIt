using System;
using System.Collections.Generic;
using System.Threading;

namespace MeasureIt.Discovery
{
    using Xunit;
    using static LazyThreadSafetyMode;

    public abstract class DiscoveryServiceTestFixtureBase<TService> : IntegrationTestFixtureBase
        where TService : class, IInstrumentationDiscoveryService
    {
        protected abstract IInstrumentationDiscoveryOptions GetDiscoveryOptions();

        protected IInstrumentationDiscoveryOptions DiscoveryOptions => GetDiscoveryOptions();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <returns></returns>
        protected delegate TService ServiceFactoryDelegate(IInstrumentationDiscoveryOptions discoveryOptions);

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
            _lazyDiscoveryService = new Lazy<TService>(() =>
                    ServiceFactory(DiscoveryOptions).VerifyDiscoveryService(OnBeforeDiscovery)
                        .VerifyDiscover().VerifyDiscoveryService(OnAfterDiscovery)
                , ExecutionAndPublication);
        }

        protected TService DiscoveryService => _lazyDiscoveryService.Value;

        [Fact]
        public void CanDiscover()
        {
            // This is it. Just handshake that the Service Is no longer Pending.
            Assert.False(DiscoveryService.IsPending);
        }

        protected abstract void VerifyDiscoveredCounterAdapters(IEnumerable<IPerformanceCounterAdapter> discoveredItems);

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
