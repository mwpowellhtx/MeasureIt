using System;

namespace MeasureIt.Web.Mvc.Castle.Windsor
{
    using Contexts;
    using Discovery;
    using Interception;
    using MeasureIt.Discovery;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    /// <summary>
    /// Registration extension methods.
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>, and using <see cref="MvcActionMeasurementProvider"/> for
        /// provider.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        /// <param name="optsCreated"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMvcMeasurements<TInterface, TService>(
            this IWindsorContainer container
            , Action<IInstrumentationDiscoveryOptions> optsCreated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
        {
            return container.EnableMvcMeasurements<TInterface, TService, MvcActionMeasurementProvider>(
                optsCreated);
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="container"></param>
        /// <param name="optsCreated"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMvcMeasurements<TInterface, TService, TProvider>(
            this IWindsorContainer container
            , Action<IInstrumentationDiscoveryOptions> optsCreated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TProvider : class, ITwoStageMeasurementProvider
        {
            optsCreated = optsCreated ?? delegate { };

            container.Register(

                Component.For<IInstrumentationDiscoveryOptions>()
                    .ImplementedBy<InstrumentationDiscoveryOptions>()
                    .LifestyleTransient().OnCreate(optsCreated)

                // TODO: TBD: will need to be careful with the lifestyle here... or the capture/usage of it in the attribute...
                , Component.For<ITwoStageMeasurementProvider>()
                    .ImplementedBy<TProvider>().LifestyleTransient()

            );

            {
                var interfaceType = typeof(TInterface);

                interfaceType.VerifyIsInterface();

                var interfaceReg = Component.For<TInterface>()
                        .ImplementedBy<TService>().LifestyleTransient()
                        .Forward<TInterface, IRuntimeInstrumentationDiscoveryService>()
                        .Forward<TInterface, IInstallerInstrumentationDiscoveryService>()
                    ;

                container.Register(interfaceReg);
            }

            return container;
        }
    }
}
