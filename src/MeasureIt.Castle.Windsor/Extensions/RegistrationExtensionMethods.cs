using System;
using System.Linq;

namespace MeasureIt.Castle.Windsor
{
    using Discovery;
    using Interception;
    using Interception.Measurement;
    using global::Castle.Core;
    using global::Castle.DynamicProxy;
    using global::Castle.MicroKernel.Registration;
    using IWindsorContainer = global::Castle.Windsor.IWindsorContainer;
    using static Discovery.InstrumentationDiscoveryOptions;

    /// <summary>
    /// 
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMeasurements<TInterface, TService>(
            this IWindsorContainer container
            , Func<InstrumentationDiscoveryOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
        {
            return container.EnableMeasurements<TInterface, TService
                , InstrumentationDiscoveryOptions
                , MeasurementInterceptor>();
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="container"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMeasurements<TInterface, TService, TOptions>(
            this IWindsorContainer container
            , Func<TOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return container.EnableMeasurements<TInterface, TService, TOptions
                , MeasurementInterceptor>();
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="container"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMeasurements<TInterface, TService, TOptions, TInterceptor>(
            this IWindsorContainer container
            , Func<TOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
            where TInterceptor : class, IMeasurementInterceptor
        {
            {
                createOptions = createOptions ?? CreateDefaultDiscoveryOptions<TOptions>;

                var optionsType = typeof(TOptions);

                optionsType.VerifyIsClass();

                container.Register(

                    Component.For(optionsType.GetInterfaces())
                        .UsingFactoryMethod(createOptions).LifestyleSingleton()

                );
            }

            container.Register(

                Component.For<IInterceptionMeasurementProvider>()
                    .ImplementedBy<InterceptionMeasurementProvider>().LifestyleTransient()

                , Component.For<IMeasurementInterceptor>()
                    .ImplementedBy<TInterceptor>().LifestyleTransient()

            );

            {
                var interfaceType = typeof(TInterface);

                interfaceType.VerifyIsInterface();

                var discoveryServiceType = typeof(IRuntimeInstrumentationDiscoveryService);

                var interfaceReg
                    = interfaceType != discoveryServiceType
                      && discoveryServiceType.IsAssignableFrom(interfaceType)
                        ? Component.For<TInterface, IRuntimeInstrumentationDiscoveryService>()
                        : Component.For<TInterface>();

                container.Register(
                    interfaceReg.ImplementedBy<TService>().LifestyleTransient()
                );
            }

            return container;
        }

        /// <summary>
        /// Remember to call
        /// <see cref="EnableMeasurements{TOptions,TInterface,TService,TInterceptor}"/> at some
        /// point during the Dependency Injection setup, following which one or more of your
        /// registered <typeparamref name="T"/> classes may be measured.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static ComponentRegistration<T> Measure<T>(this ComponentRegistration<T> registration)
            where T : class
        {
            return registration.MeasureUsing<T, IMeasurementInterceptor>();
        }

        /// <summary>
        /// Remember to call
        /// <see cref="EnableMeasurements{TOptions,TInterface,TService,TInterceptor}"/> at some
        /// point during the Dependency Injection setup, following which one or more of your
        /// registered <typeparamref name="T"/> classes may be measured.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static ComponentRegistration<T> MeasureUsing<T, TInterceptor>(this ComponentRegistration<T> registration)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            return registration.Interceptors(InterceptorReference.ForType<TInterceptor>()).Anywhere;
        }

        /// <summary>
        /// Measure the <paramref name="obj"/> Instance providing for <paramref name="container"/>
        /// and <paramref name="createGeneratorOptions"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="obj"></param>
        /// <param name="createGeneratorOptions"></param>
        /// <returns></returns>
        public static T MeasureInstance<T>(this IWindsorContainer container
            , T obj, Func<ProxyGenerationOptions> createGeneratorOptions = null)
            where T : class
        {
            return container.MeasureInstance<T, MeasurementInterceptor>(obj, createGeneratorOptions);
        }

        /// <summary>
        /// Measure the <paramref name="obj"/> Instance providing for <paramref name="container"/>
        /// and <paramref name="createGeneratorOptions"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="container"></param>
        /// <param name="obj"></param>
        /// <param name="createGeneratorOptions"></param>
        /// <returns></returns>
        public static T MeasureInstance<T, TInterceptor>(this IWindsorContainer container
            , T obj, Func<ProxyGenerationOptions> createGeneratorOptions = null)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            createGeneratorOptions = createGeneratorOptions ?? (() => new ProxyGenerationOptions());

            var opts = createGeneratorOptions();

            // TODO: ModuleScope?
            var generator = new ProxyGenerator();

            var interceptors = container.ResolveAll<TInterceptor>().ToArray<IInterceptor>();

            var proxy = generator.CreateClassProxyWithTarget(obj, opts, interceptors);

            return proxy;
        }
    }
}
