using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    using Autofac;
    using Autofac.Extras.DynamicProxy;
    using Castle.DynamicProxy;
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TImplementer"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="builder"></param>
        /// <param name="optsProxyGeneration"></param>
        /// <returns></returns>
        public static ContainerBuilder EnableMeasurementInterception<TImplementer, TService, TInterceptor>(
            this ContainerBuilder builder, Action<AutofacProxyGenerationOptions> optsProxyGeneration = null)
            where TImplementer : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsProxyGeneration = optsProxyGeneration ?? delegate { };

            var opts = new AutofacProxyGenerationOptions();

            optsProxyGeneration(opts);

            // TODO: TBD: may want to derive this "feature" based on TImplementer/TService types instead...
            const AutofacEnableInterceptionOption cls = AutofacEnableInterceptionOption.Class;
            const AutofacEnableInterceptionOption intf = AutofacEnableInterceptionOption.Interface;

            if (!((opts.EnableInterception & cls) == cls
                || (opts.EnableInterception & intf) == intf))
            {
                const string message = "Expected an option for Autofac EnableInterception.";
                throw new InvalidOperationException(message);
            }

            var interceptorType = typeof(TInterceptor);

            var regBuilder = builder
                .RegisterType<TImplementer>()
                .As<TService>();

            // TODO: TBD: whether to capture enable xyz interceptors (Type[] additionalInterfaces) args via options
            if ((opts.EnableInterception & cls) == cls)
            {
                regBuilder.EnableClassInterceptors(opts);
            }

            if ((opts.EnableInterception & intf) == intf)
            {
                regBuilder.EnableInterfaceInterceptors(opts);
            }

            regBuilder.InterceptedBy(interceptorType);

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
            , T obj, Action<AutofacProxyGenerationOptions> optsProxyGeneration = null)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            optsProxyGeneration = optsProxyGeneration ?? delegate { };

            var opts = new AutofacProxyGenerationOptions();

            optsProxyGeneration(opts);

            // TODO: ModuleScope?
            var generator = new ProxyGenerator();

            var interceptors = container.Resolve<IEnumerable<TInterceptor>>().ToArray<IInterceptor>();

            var proxy = generator.CreateClassProxyWithTarget(obj, opts, interceptors);

            return proxy;
        }
    }
}
