using System;
using System.Linq;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using IWindsorContainer = Castle.Windsor.IWindsorContainer;

namespace MeasureIt.Castle.Windsor
{
    using Discovery;
    using Interception;
    using Interception.Measurement;

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
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="container"></param>
        /// <param name="optsCreated"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableMeasurements<TInterface, TService, TInterceptor>(
            this IWindsorContainer container
            , Action<IInstrumentationDiscoveryOptions> optsCreated = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsCreated = optsCreated ?? (o => { });

            container.Register(

                Component.For<IInstrumentationDiscoveryOptions>()
                    .ImplementedBy<InstrumentationDiscoveryOptions>()
                    .LifestyleTransient().OnCreate(optsCreated)

                , Component.For<IInterceptionMeasurementProvider>()
                    .ImplementedBy<InterceptionMeasurementProvider>().LifestyleTransient()

                , Component.For<IMeasurementInterceptor>()
                    .ImplementedBy<TInterceptor>().LifestyleTransient()

                );

            {
                var interfaceType = typeof(TInterface);

                interfaceType.VerifyIsInterface();

                var interfaceReg = Component.For<TInterface>()
                    .ImplementedBy<TService>().LifestyleTransient();

                var discoveryServiceType = typeof(IRuntimeInstrumentationDiscoveryService);

                // Register the Runtime Instrumentation when necessary.
                if (interfaceType != discoveryServiceType
                    && discoveryServiceType.IsAssignableFrom(interfaceType))
                {
                    // IRuntimeInstrumentationDiscoveryService is required by InterceptionMeasurementProvider.
                    interfaceReg.Forward<TInterface, IRuntimeInstrumentationDiscoveryService>();
                }

                container.Register(interfaceReg);
            }

            return container;
        }

        /// <summary>
        /// Remember to call <see cref="EnableMeasurements{TInterface, TService, TInterceptor}"/>
        /// at some point during the Dependency Injection setup, following which one or more of
        /// your registered <typeparamref name="T"/> classes may be measured.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static ComponentRegistration<T> MeasureUsing<T, TInterceptor>(
            this ComponentRegistration<T> registration)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            return registration.Interceptors(InterceptorReference.ForType<TInterceptor>()).Anywhere;
        }

        /// <summary>
        /// Measure the <paramref name="obj"/> Instance providing for <paramref name="container"/>
        /// and <paramref name="optsProxyGeneration"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="container"></param>
        /// <param name="obj"></param>
        /// <param name="optsProxyGeneration"></param>
        /// <returns></returns>
        public static T MeasureInstance<T, TInterceptor>(this IWindsorContainer container
            , T obj, Action<ProxyGenerationOptions> optsProxyGeneration = null)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsProxyGeneration = optsProxyGeneration ?? (o => { });

            var opts = new ProxyGenerationOptions();

            optsProxyGeneration(opts);

            // TODO: ModuleScope?
            var generator = new ProxyGenerator();

            var interceptors = container.ResolveAll<TInterceptor>().ToArray<IInterceptor>();

            var proxy = generator.CreateClassProxyWithTarget(obj, opts, interceptors);

            return proxy;
        }
    }
}
