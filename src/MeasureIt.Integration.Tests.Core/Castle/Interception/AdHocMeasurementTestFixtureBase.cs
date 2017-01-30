using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Castle.Interception
{
    using Discovery;
    using Measurement;
    using Xunit;
    using global::Castle.DynamicProxy;

    public abstract class AdHocMeasurementTestFixtureBase<T, TInterceptor> : DisposableTestFixtureBase
        where T : class, new()
        where TInterceptor : MeasurementInterceptor
    {
        protected static T Get()
        {
            var g = new T();
            Assert.NotNull(g);
            return g;
        }

        protected virtual IInstrumentationDiscoveryOptions CreateDiscoveryOptions()
        {
            var o = new InstrumentationDiscoveryOptions {ThrowOnUninstallerFailure = false};
            Assert.NotNull(o);
            return o;
        }

        private readonly Lazy<IInstrumentationDiscoveryOptions> _lazyDiscoveryOptions;

        private readonly Lazy<IInstallerInstrumentationDiscoveryService> _lazyDiscoveryService;

        private readonly Lazy<IInterceptionMeasurementProvider> _lazyMeasurementProvider;

        protected IInterceptionMeasurementProvider MeasurementProvider => _lazyMeasurementProvider.Value;

        private static TService VerifyServiceDiscovery<TService>(TService service)
            where TService : IInstrumentationDiscoveryService
        {
            // We are not here to test that
            bool? discovered = null;
            service.Discovered += delegate { discovered = true; };
            service.Discover();
            Assert.True(discovered);
            return service;
        }

        private IInstallerInstrumentationDiscoveryService CreateDiscoveryService()
        {
            var discoveryOptions = _lazyDiscoveryOptions.Value;

            // Verify a couple of things just prior to usage.
            Assert.False(discoveryOptions.ThrowOnUninstallerFailure);
            Assert.True(discoveryOptions.ThrowOnInstallerFailure);

            var service = new InstallerInstrumentationDiscoveryService(discoveryOptions);

            Assert.NotNull(service);

            return VerifyServiceDiscovery(service);
        }

        private IInterceptionMeasurementProvider CreateMeasurementProvider()
        {
            var discoveryOptions = _lazyDiscoveryOptions.Value;
            var discoveryService = _lazyDiscoveryService.Value;

            var provider = new InterceptionMeasurementProvider(discoveryOptions, discoveryService);

            Assert.NotNull(provider);

            return provider;
        }

        protected abstract TInterceptor CreateInterceptor();

        private void Install()
        {
            using (var context = _lazyDiscoveryService.Value.GetInstallerContext())
            {
                context.Install();
            }
        }

        protected AdHocMeasurementTestFixtureBase()
        {
            _lazyDiscoveryOptions = new Lazy<IInstrumentationDiscoveryOptions>(CreateDiscoveryOptions);
            _lazyDiscoveryService = new Lazy<IInstallerInstrumentationDiscoveryService>(CreateDiscoveryService);
            _lazyMeasurementProvider = new Lazy<IInterceptionMeasurementProvider>(CreateMeasurementProvider);

            Install();
        }

        private void Uninstall()
        {
            using (var context = _lazyDiscoveryService.Value.GetInstallerContext())
            {
                context.Uninstall();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || IsDisposed) return;

            Uninstall();

            base.Dispose(true);
        }
    }

    public abstract class AdHocMeasurementTestFixtureBase<T> :
        AdHocMeasurementTestFixtureBase<T, MeasurementInterceptorFixture>
        where T : class, new()
    {
        private bool? _intercepted;

        private bool? _measuring;

        private bool? _measured;

        protected virtual void SubjectClass_Intercepted(object sender, InvocationInterceptedEventArgs e)
        {
            _intercepted = true;
        }

        protected virtual void SubjectClass_Measuring(object sender, InvocationInterceptedEventArgs e)
        {
            _measuring = true;
        }

        protected virtual void SubjectClass_Measured(object sender, InvocationInterceptedEventArgs e)
        {
            _measured = true;
        }

        private Action<T> Invoke { get; }

        protected AdHocMeasurementTestFixtureBase(Action<T> invoke)
        {
            Assert.NotNull(invoke);
            Invoke = invoke;
            MeasurementInterceptorFixture.Intercepted += SubjectClass_Intercepted;
            MeasurementInterceptorFixture.Measuring += SubjectClass_Measuring;
            MeasurementInterceptorFixture.Measured += SubjectClass_Measured;
        }

        protected override MeasurementInterceptorFixture CreateInterceptor()
        {
            var fixture = new MeasurementInterceptorFixture(MeasurementProvider);
            Assert.NotNull(fixture);
            return fixture;
        }

        private IEnumerable<IInterceptor> GetInterceptors()
        {
            yield return CreateInterceptor();
        }

        [Fact]
        public void Verify()
        {
            var g = Get();

            var m = g.MeasureSingleInstance(interceptors: GetInterceptors().ToArray());

            Invoke(m);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || IsDisposed) return;

            /* Whether successful or not does not mean much at this moment.
             *  So take care of unhooking the events. */

            MeasurementInterceptorFixture.Intercepted -= SubjectClass_Intercepted;
            MeasurementInterceptorFixture.Measuring -= SubjectClass_Measuring;
            MeasurementInterceptorFixture.Measured -= SubjectClass_Measured;

            // Then verify the results.
            Assert.True(_intercepted);
            Assert.True(_measuring);
            Assert.True(_measured);
        }
    }
}
