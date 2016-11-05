using System;

namespace MeasureIt.Castle.Windsor
{
    using Contexts;
    using Discovery;
    using Interception.Measurement;
    using Xunit;

    /// <summary>
    /// When we include the <see cref="InstallerInstrumentationDiscoveryService"/> in the mix,
    /// we also pick up the <see cref="RuntimeInstrumentationDiscoveryService"/>. We include the
    /// <see cref="MeasurementInterceptorFixture"/> for purposes of connecting test results with
    /// interception messages.
    /// </summary>
    public abstract class InstallerMeasurementInterceptorTestFixtureBase : DependencyInjectionTestFixtureBase<
        IInstallerInstrumentationDiscoveryService, InstallerInstrumentationDiscoveryService>
    {
        protected override void InitializeOptions(IInstrumentationDiscoveryOptions options)
        {
            base.InitializeOptions(options);

            // Should throw under normal circumstances, but not here.
            options.ThrowOnInstallerFailure = false;
        }

        private void RunInstallerContext(Action<IInstallerContext> action)
        {
            Assert.NotNull(action);

            using (var context = DiscoveryService.GetInstallerContext())
            {
                action(context);
            }
        }

        protected InstallerMeasurementInterceptorTestFixtureBase()
        {
            RunInstallerContext(context => context.Install());
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                RunInstallerContext(context => context.Uninstall());
            }

            base.Dispose(disposing);
        }
    }
}
