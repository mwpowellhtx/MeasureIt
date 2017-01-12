using System;

namespace MeasureIt.Castle
{
    using Discovery;
    using Xunit;

    /// <summary>
    /// When we include the <see cref="InstallerInstrumentationDiscoveryService"/> in the mix, we
    /// also pick up the <see cref="RuntimeInstrumentationDiscoveryService"/>. We include the
    /// <see cref="Interception.Measurement.MeasurementInterceptorFixture"/> for purposes of
    /// connecting test results with interception messages.
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TOptions">We expose options in this manner because we may build upon the
    /// options for some projects.</typeparam>
    public abstract class InstallerMeasurementInterceptorTestFixtureBase<TContainer, TOptions>
        : DependencyInjectionTestFixtureBase<IInstallerInstrumentationDiscoveryService, TContainer, TOptions>
        where TContainer : class
        where TOptions : class, IInstrumentationDiscoveryOptions, new()
    {
        private void UseDiscoveryService(Action<IInstallerInstrumentationDiscoveryService> action)
        {
            Assert.NotNull(action);
            action(DiscoveryService);
        }

        protected override TOptions GetDiscoveryOptions()
        {
            var options = base.GetDiscoveryOptions();

            // TODO: TBD: ???
            // Should throw under normal circumstances, but not here.
            options.ThrowOnInstallerFailure = true;

            return options;
        }

        protected InstallerMeasurementInterceptorTestFixtureBase()
        {
            var o = DiscoveryOptions;

            UseDiscoveryService(ds =>
            {
                Assert.True(ds.TryUninstall(o));
                ds.Install(o);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                UseDiscoveryService(ds => Assert.True(ds.TryUninstall(DiscoveryOptions)));
            }

            base.Dispose(disposing);
        }
    }
}
