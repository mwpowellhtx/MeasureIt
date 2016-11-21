using System;

namespace MeasureIt.Castle
{
    using Discovery;
    using Interception.Measurement;
    using Xunit;

    /// <summary>
    /// When we include the <see cref="InstallerInstrumentationDiscoveryService"/> in the mix,
    /// we also pick up the <see cref="RuntimeInstrumentationDiscoveryService"/>. We include the
    /// <see cref="MeasurementInterceptorFixture"/> for purposes of connecting test results with
    /// interception messages.
    /// </summary>
    public abstract class InstallerMeasurementInterceptorTestFixtureBase<TContainer>
        : DependencyInjectionTestFixtureBase<IInstallerInstrumentationDiscoveryService, TContainer>
        where TContainer : class
    {
        protected override void InitializeOptions(IInstrumentationDiscoveryOptions options)
        {
            base.InitializeOptions(options);

            // TODO: TBD: ???
            // Should throw under normal circumstances, but not here.
            options.ThrowOnInstallerFailure = true;
        }

        private void UseDiscoveryService(Action<IInstallerInstrumentationDiscoveryService> action)
        {
            Assert.NotNull(action);
            action(DiscoveryService);
        }

        protected InstallerMeasurementInterceptorTestFixtureBase()
        {
            UseDiscoveryService(ds =>
            {
                Assert.True(ds.TryUninstall());
                ds.Install();
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                UseDiscoveryService(ds => Assert.True(ds.TryUninstall()));
            }

            base.Dispose(disposing);
        }
    }
}
