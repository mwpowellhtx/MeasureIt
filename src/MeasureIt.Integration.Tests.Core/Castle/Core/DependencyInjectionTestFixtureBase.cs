using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MeasureIt.Castle
{
    using Discovery;
    using Interception.Measurement;
    using Xunit;
    using static LazyThreadSafetyMode;

    /// <summary>
    /// Establishes a base dependency injection test fixture representing
    /// <typeparamref name="TInterface"/>, <typeparamref name="TContainer"/>, and
    /// <typeparamref name="TOptions"/> concerns.
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TOptions">We expose options in this manner because we may build upon the
    /// options for some projects.</typeparam>
    public abstract class DependencyInjectionTestFixtureBase<TInterface, TContainer, TOptions> : Disposable
        where TInterface : class, IInstallerInstrumentationDiscoveryService
        where TContainer : class
        where TOptions : class, IInstrumentationDiscoveryOptions, new()
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

        private readonly Lazy<TContainer> _lazyContainer;

        protected TContainer Container => _lazyContainer.Value;

        /// <summary>
        /// Performs default <typeparamref name="TContainer"/> initialization.
        /// Default does nothing.
        /// </summary>
        protected virtual void InitializeContainer()
        {
        }

        private readonly Lazy<TInterface> _lazyDiscoveryService;

        protected TInterface DiscoveryService => _lazyDiscoveryService.Value;

        protected virtual IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IDescriptor).Assembly;
            yield return typeof(Support.Root).Assembly;
        }

        protected virtual TOptions GetDiscoveryOptions()
        {
            return new TOptions {Assemblies = GetAssemblies().ToArray()};
        }

        protected TOptions DiscoveryOptions => GetDiscoveryOptions();

        protected abstract TContainer GetContainer();

        protected abstract TInterface GetInterface();

        private void Initialize()
        {
            InitializeContainer();
        }

        protected abstract SubjectClass GetMeasuredSubject(SubjectClass obj);

        protected SubjectClass GetSubject()
        {
            var obj = new SubjectClass();

            var measured = GetMeasuredSubject(obj);

            Assert.NotNull(measured);

            Assert.NotEqual(obj.GetType(), measured.GetType());

            // ReSharper disable once UseMethodIsInstanceOfType
            // There should be no question about this at this point...
            Assert.True(obj.GetType().IsAssignableFrom(measured.GetType()));

            // Having tested the Types, that is no mistake, since we are expecting a Proxy.
            return measured;
        }

        /// <summary>
        /// Protected Constructor that also Installs the DiscoveryService PerformanceCounterCategory(ies).
        /// </summary>
        protected DependencyInjectionTestFixtureBase()
        {
            Initialize();

            _lazyContainer = new Lazy<TContainer>(GetContainer);

            _lazyDiscoveryService = new Lazy<TInterface>(GetInterface, ExecutionAndPublication);
        }

        protected virtual void OnIntercepted(object sender, InvocationInterceptedEventArgs e)
        {
        }
    }
}
