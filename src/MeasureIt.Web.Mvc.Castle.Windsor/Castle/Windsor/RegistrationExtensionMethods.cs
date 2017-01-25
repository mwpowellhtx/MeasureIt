using System;

namespace MeasureIt.Web.Mvc.Castle.Windsor
{
    using Contexts;
    using Discovery;
    using Interception;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;
    using static Discovery.InstrumentationDiscoveryOptions;

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
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="container"></param>
        /// <param name="createOptions"></param>
        /// <param name="optionsCreated"></param>
        /// <returns></returns>
        /// <see cref="EnableMvcMeasurements{TInterface,TService,TOptions,TProvider}"/>
        public static IWindsorContainer EnableMvcMeasurements<TInterface, TService, TOptions>(
            this IWindsorContainer container
            , Func<TOptions> createOptions = null
            , Action<TOptions> optionsCreated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return container.EnableMvcMeasurements<TInterface, TService, TOptions
                , MvcActionMeasurementProvider>(createOptions, optionsCreated);
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="container"></param>
        /// <param name="createOptions"></param>
        /// <param name="optionsCreated"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMvcMeasurements<TInterface, TService, TOptions, TProvider>(
            this IWindsorContainer container
            , Func<TOptions> createOptions = null
            , Action<TOptions> optionsCreated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
            where TProvider : class, ITwoStageMeasurementProvider
        {
            createOptions = createOptions ?? CreateDefaultDiscoveryOptions<TOptions>;
            optionsCreated = optionsCreated ?? delegate { };

            container.Register(

                Component.For(typeof(TOptions).GetInterfaces())
                    .UsingFactoryMethod(createOptions)
                    .OnCreate(obj => optionsCreated((TOptions) obj))
                    .LifestyleSingleton()

                // TODO: TBD: will need to be careful with the lifestyle here... or the capture/usage of it in the attribute...
                , Component.For<ITwoStageMeasurementProvider>()
                    .ImplementedBy<TProvider>().LifestyleTransient()

            );

            {
                typeof(TInterface).VerifyIsInterface();

                container.Register(
                    
                    // TODO: TBD: just register all of the implemented interfaces?
                    Component.For<TInterface
                            , IRuntimeInstrumentationDiscoveryService
                            , IInstallerInstrumentationDiscoveryService>()
                        .ImplementedBy<TService>().LifestyleTransient()

                );
            }

            return container;
        }
    }
}
