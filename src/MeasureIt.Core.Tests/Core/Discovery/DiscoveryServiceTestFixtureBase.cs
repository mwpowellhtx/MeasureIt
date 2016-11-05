using System;
using System.Collections.Generic;

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
            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.Empty(service.CounterAdapterDescriptors);
        }

        protected virtual void OnAfterDiscovery(TService service)
        {
            Assert.False(service.IsPending);

            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.NotEmpty(service.CounterAdapterDescriptors);

            OnVerifyCounterAdapterDescriptors(service.CounterAdapterDescriptors);
        }

        private readonly Lazy<TService> _lazyDiscoveryService;

        protected DiscoveryServiceTestFixtureBase()
        {
            _lazyDiscoveryService = new Lazy<TService>(() =>
                ServiceFactory(Options).VerifyDiscoveryService(OnBeforeDiscovery)
                    .VerifyDiscover().VerifyDiscoveryService(OnAfterDiscovery)
                );
        }

        protected TService DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

        protected virtual void OnVerifyCounterAdapterDescriptors(
            IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors)
        {
            descriptors.Verify();
        }

        //// TODO: TBD: consider re-factoring this for the moment at which discovery service is referenced for the first time, as part of its initialization sequence...
        //protected virtual TService GetDiscoveredDiscoveryService()
        //{
        //    var discoveryService = DiscoveryService;

        //    // TODO: TBD: may want to initialize the collections up front...
        //    Assert.True(discoveryService.IsPending);
        //    OnBeforeDiscovery(discoveryService);

        //    discoveryService.Discover();
            
        //    Assert.False(discoveryService.IsPending);
        //    OnAfterDiscovery(discoveryService);

        //    return discoveryService;
        //}

        [Fact]
        public void CanDiscover()
        {
            // This is it. Just handshake that the Service Is no longer Pending.
            Assert.False(DiscoveryService.IsPending);
        }

        protected abstract void VerifyDiscoveredCounterAdapterDescriptors(
            IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors);

        protected virtual void OnVerifyDescriptors(TService service)
        {
            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.NotEmpty(service.CounterAdapterDescriptors);

            VerifyDiscoveredCounterAdapterDescriptors(service.CounterAdapterDescriptors);
        }

        [Fact]
        public void VerifyDescriptors()
        {
            OnVerifyDescriptors(DiscoveryService);
        }
    }
}
