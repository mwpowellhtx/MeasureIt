﻿using System;

namespace MeasureIt.Web.Mvc.Autofac
{
    using Contexts;
    using Discovery;
    using Interception;
    using global::Autofac;
    using global::Autofac.Builder;
    using static Discovery.InstrumentationDiscoveryOptions;

    /// <summary>
    /// Registration extension methods.
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// Registers the <typeparamref name="TService"/> as the
        /// <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="builder"></param>
        /// <returns>Returns the registration or further consideration.</returns>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData
            , SingleRegistrationStyle> RegisterService<TInterface, TService>(
            this ContainerBuilder builder)
            where TInterface : class
            where TService : class, TInterface
        {
            typeof(TInterface).VerifyIsInterface();
            typeof(TService).VerifyIsClass();

            return builder.RegisterType<TService>().As<TInterface>();
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="builder"/>, and using <see cref="MvcActionMeasurementProvider"/> for
        /// provider.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <param name="optionsActivated"></param>
        /// <returns></returns>
        /// <see cref="EnableMvcMeasurements{TInterface,TService,TOptions,TProvider}"/>
        public static ContainerBuilder EnableMvcMeasurements<TInterface, TService>(
            this ContainerBuilder builder
            , Func<InstrumentationDiscoveryOptions> createOptions = null
            , Action<InstrumentationDiscoveryOptions> optionsActivated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
        {
            return builder.EnableMvcMeasurements<TInterface, TService
                , InstrumentationDiscoveryOptions
                , MvcActionMeasurementProvider>(createOptions, optionsActivated);
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="builder"/>, and using <see cref="MvcActionMeasurementProvider"/> for
        /// provider.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <param name="optionsActivated"></param>
        /// <returns></returns>
        /// <see cref="EnableMvcMeasurements{TInterface,TService,TOptions,TProvider}"/>
        public static ContainerBuilder EnableMvcMeasurements<TInterface, TService, TOptions>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null
            , Action<TOptions> optionsActivated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return builder.EnableMvcMeasurements<TInterface, TService, TOptions
                , MvcActionMeasurementProvider>(createOptions, optionsActivated);
        }

        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="builder"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <param name="optionsActivated"></param>
        /// <returns></returns>
        public static ContainerBuilder EnableMvcMeasurements<TInterface, TService, TOptions, TProvider>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null
            , Action<TOptions> optionsActivated = null)
            where TInterface : class, IMvcActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
            where TProvider : class, ITwoStageMeasurementProvider
        {
            {
                typeof(TOptions).VerifyIsClass();

                createOptions = createOptions ?? CreateDefaultDiscoveryOptions<TOptions>;
                optionsActivated = optionsActivated ?? delegate { };

                builder.Register(context => createOptions())
                    .AsImplementedInterfaces()
                    .SingleInstance()
                    .OnActivated(args => optionsActivated(args.Instance));
            }

            // TODO: TBD: should this accept the full Activated event handler? doubtful, but we'll see...

            builder.RegisterType<TProvider>()
                .AsImplementedInterfaces()
                .InstancePerRequest();

            {
                typeof(TService).VerifyIsClass();
                typeof(TInterface).VerifyIsInterface();

                builder.RegisterType<TService>()
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            return builder;
        }
    }
}
