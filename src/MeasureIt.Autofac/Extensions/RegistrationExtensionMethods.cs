using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    using Autofac;
    using Autofac.Extras.DynamicProxy;
    using Castle.Interception;
    using Castle.Interception.Measurement;
    using Discovery;
    using global::Castle.DynamicProxy;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>We also need to make a reference to <see cref="IInterceptor"/>.</remarks>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="builder"></param>
        /// <param name="optsCreated"></param>
        /// <returns></returns>
        public static ContainerBuilder EnableMeasurements<TInterface, TService, TInterceptor>(
            this ContainerBuilder builder
            , Action<IInstrumentationDiscoveryOptions> optsCreated = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsCreated = optsCreated ?? (delegate { });

            {
                builder.RegisterType<InstrumentationDiscoveryOptions>()
                    .As<IInstrumentationDiscoveryOptions>()
                    .InstancePerDependency()
                    .OnActivated(args => optsCreated(args.Instance));

                builder.RegisterType<InterceptionMeasurementProvider>()
                    .As<IInterceptionMeasurementProvider>()
                    .InstancePerDependency();

                builder.RegisterType<TInterceptor>()
                    .As<IMeasurementInterceptor>()
                    .InstancePerDependency();
            }

            {
                var interfaceType = typeof(TInterface);

                interfaceType.VerifyIsInterface();

                var serviceReg = builder.RegisterType<TService>()
                    .As<TInterface>()
                    .InstancePerDependency();

                var discoveryServiceType = typeof(IRuntimeInstrumentationDiscoveryService);

                // Register the Runtime Instrumentation when necessary.
                if (interfaceType != discoveryServiceType
                    && discoveryServiceType.IsAssignableFrom(interfaceType))
                {
                    // IRuntimeInstrumentationDiscoveryService is required by InterceptionMeasurementProvider.
                    serviceReg.As<IRuntimeInstrumentationDiscoveryService>();
                }
            }

            return builder;
        }

        public static ContainerBuilder EnableClassInterception<TImplementer, TService, TInterceptor>(
            this ContainerBuilder builder, Action<ProxyGenerationOptions> optsProxyGeneration = null)
            where TImplementer : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsProxyGeneration = optsProxyGeneration ?? delegate { };

            var opts = new ProxyGenerationOptions();

            optsProxyGeneration(opts);

            var interceptorType = typeof(TInterceptor);

            builder
                .RegisterType<TImplementer>()
                .As<TService>()
                .EnableClassInterceptors(opts)
                .InterceptedBy(interceptorType);

            return builder;
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
        public static T MeasureInstance<T, TInterceptor>(this IContainer container
            , T obj, Action<ProxyGenerationOptions> optsProxyGeneration = null)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsProxyGeneration = optsProxyGeneration ?? delegate { };

            var opts = new ProxyGenerationOptions();

            optsProxyGeneration(opts);

            // TODO: ModuleScope?
            var generator = new ProxyGenerator();

            var interceptors = container.Resolve<IEnumerable<TInterceptor>>().ToArray<IInterceptor>();

            var proxy = generator.CreateClassProxyWithTarget(obj, opts, interceptors);

            return proxy;
        }
    }
}
