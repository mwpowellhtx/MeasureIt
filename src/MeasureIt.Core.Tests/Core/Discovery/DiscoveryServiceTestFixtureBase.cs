using System;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public abstract class DiscoveryServiceTestFixtureBase<TService> : IntegrationTestFixtureBase
        where TService : class, IInstrumentationDiscoveryService
    {
        private readonly Lazy<TService> _lazyDiscoveryService;

        private static TService VerifyCreatedService(TService service)
        {
            Assert.NotNull(service);
            Assert.True(service.IsPending);
            Assert.NotNull(service.CounterAdapterDescriptors);
            Assert.Empty(service.CounterAdapterDescriptors);
            return service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        protected delegate TService ServiceFactoryDelegate(InstrumentationDiscovererOptions options,
            IEnumerable<Assembly> assemblies);

        protected DiscoveryServiceTestFixtureBase(InstrumentationDiscovererOptions options,
            IEnumerable<Assembly> assemblies, ServiceFactoryDelegate serviceFactory)
        {
            Assert.NotNull(assemblies);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.NotEmpty(assemblies);

            // ReSharper disable once PossibleMultipleEnumeration
            _lazyDiscoveryService = new Lazy<TService>(() => VerifyCreatedService(serviceFactory(options, assemblies)));
        }

        private TService DiscoveryService
        {
            get
            {
                var result = _lazyDiscoveryService.Value;
                Assert.NotNull(result);
                return result;
            }
        }

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

        protected virtual void OnVerifyCounterAdapterDescriptors(
            IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors)
        {
            descriptors.Verify();
        }

        protected virtual TService GetDiscoveredDiscoveryService()
        {
            var discoveryService = DiscoveryService;

            // TODO: TBD: may want to initialize the collections up front...
            Assert.True(discoveryService.IsPending);
            OnBeforeDiscovery(discoveryService);

            discoveryService.Discover();
            
            Assert.False(discoveryService.IsPending);
            OnAfterDiscovery(discoveryService);

            return discoveryService;
        }

        [Fact]
        public void CanDiscover()
        {
            GetDiscoveredDiscoveryService();
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
            var discoveryService = GetDiscoveredDiscoveryService();

            OnVerifyDescriptors(discoveryService);
        }
    }
}
