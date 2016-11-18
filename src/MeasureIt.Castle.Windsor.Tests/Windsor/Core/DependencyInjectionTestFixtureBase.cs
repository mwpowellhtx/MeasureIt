using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Castle.Windsor
{
    using Discovery;
    using Interception.Measurement;
    using Xunit;
    using global::Castle.Windsor;

    public abstract class DependencyInjectionTestFixtureBase<TInterface, TService> : Disposable
        where TInterface : class, IInstallerInstrumentationDiscoveryService
        where TService : class, TInterface
    {
        // TODO: TBD: "observables" is likely what we really want here, but this should work...
        protected class InvocationInterceptedContext : Disposable
        {
            private readonly EventHandler<InvocationInterceptedEventArgs> _onIntercepted;

            private readonly EventHandler<InvocationInterceptedEventArgs> _onMeasuring;

            private readonly EventHandler<InvocationInterceptedEventArgs> _onMeasured;

            private readonly EventHandler<InvocationInterceptedEventArgs> _defaultInvocationInterceptedHandler
                = (sender, e) => { };

            internal InvocationInterceptedContext(
                EventHandler<InvocationInterceptedEventArgs> onIntercepted = null
                , EventHandler<InvocationInterceptedEventArgs> onMeasuring = null
                , EventHandler<InvocationInterceptedEventArgs> onMeasured = null
                )
            {
                _onIntercepted = onIntercepted ?? _defaultInvocationInterceptedHandler;
                _onMeasuring = onMeasuring ?? _defaultInvocationInterceptedHandler;
                _onMeasured = onMeasured ?? _defaultInvocationInterceptedHandler;

                MeasurementInterceptorFixture.Intercepted += _onIntercepted;
                MeasurementInterceptorFixture.Measuring += _onMeasuring;
                MeasurementInterceptorFixture.Measured += _onMeasured;
            }

            protected override void Dispose(bool disposing)
            {
                if (!IsDisposed && disposing)
                {
                    MeasurementInterceptorFixture.Intercepted -= _onIntercepted;
                    MeasurementInterceptorFixture.Measuring -= _onMeasuring;
                    MeasurementInterceptorFixture.Measured -= _onMeasured;
                }

                base.Dispose(disposing);
            }
        }

        private readonly Lazy<TInterface> _lazyDiscoveryService;

        protected TInterface DiscoveryService
        {
            get { return _lazyDiscoveryService.Value; }
        }

        protected IWindsorContainer Container { get; private set; }

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IDescriptor).Assembly;
            yield return typeof(Support.Root).Assembly;
        }

        protected virtual void InitializeOptions(IInstrumentationDiscoveryOptions options)
        {
            options.Assemblies = GetAssemblies().ToArray();
        }

        /// <summary>
        /// Protected Constructor that also Installs the DiscoveryService PerformanceCounterCategory(ies).
        /// </summary>
        protected DependencyInjectionTestFixtureBase()
        {
            Container = new WindsorContainer()
                .EnableMeasurements<TInterface, TService
                    , MeasurementInterceptorFixture>(InitializeOptions);

            const LazyThreadSafetyMode execAndPubThreadSafety = LazyThreadSafetyMode.ExecutionAndPublication;

            _lazyDiscoveryService = new Lazy<TInterface>(() => Container.Resolve<TInterface>(), execAndPubThreadSafety);
        }

        protected virtual void OnIntercepted(object sender, InvocationInterceptedEventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                Container.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
