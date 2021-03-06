﻿using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

// ReSharper disable once CheckNamespace

namespace MeasureIt.Web.Http.Autofac
{
    using Contexts;
    using Discovery;
    using Interception;
    using global::Autofac;
    using global::Autofac.Builder;
    using global::Autofac.Integration.WebApi;
    using static Discovery.InstrumentationDiscoveryOptions;

    /// <summary>
    /// Most of the registration with <see cref="ContainerBuilder"/> is done via the open source
    /// <see cref="!:http://github.com/autofac/Autofac.WebApi/" /> as deployed via <see
    /// cref="!:http://www.nuget.org/packages/Autofac.WebApi2/" />. However there are a couple of
    /// extra bits that can be adjusted. Chief among them is to enable measurement via <see
    /// cref="EnableApiMeasurements{TInterface,TService,TProvider}"/>.
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// Registers the <typeparamref name="TService"/> as a kind of
        /// <typeparamref name="TInterface"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<TService, ConcreteReflectionActivatorData, SingleRegistrationStyle>
            RegisterApiService<TInterface, TService>(this ContainerBuilder builder)
            where TInterface : class
            where TService : class, TInterface
        {
            return builder.RegisterType<TService>().As<TInterface>();
        }

        /// <summary>
        /// <see cref="ServicesContainer.Replace"/> the type of <typeparamref name="TInterface"/>
        /// found in the <see cref="HttpConfiguration.Services"/> with the resolved
        /// <typeparamref name="TService"/>. Service may be a concrete class, but more likely it
        /// should be the interface that was registered against the <see cref="ContainerBuilder"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="config"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public static HttpConfiguration ReplaceService<TInterface, TService>(this HttpConfiguration config,
            IContainer container)
            where TInterface : class
            where TService : class, TInterface
        {
            // TODO: TBD: may need/want to work with a BeginLifetimeScope/ILifetimeScope instead...
            config.Services.Replace(typeof(TInterface), container.Resolve<TService>());
            return config;
        }

#pragma warning disable 612

        /// <summary>
        /// Registers services with the <paramref name="builder"/> with default
        /// <see cref="AutofacWebApiDependencyResolver"/> and
        /// <see cref="AutofacHttpControllerActivator"/> implementations provided.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterApiServices(this ContainerBuilder builder)
        {
            return builder.RegisterApiServices<
                AutofacWebApiDependencyResolver
                , AutofacHttpControllerActivator>();
        }

#pragma warning restore 612

        /// <summary>
        /// Registers services with the <paramref name="builder"/>, including
        /// <typeparamref name="TResolver"/> and <typeparamref name="TActivator"/>.
        /// </summary>
        /// <typeparam name="TResolver"></typeparam>
        /// <typeparam name="TActivator"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterApiServices<TResolver, TActivator>(
            this ContainerBuilder builder)
            where TResolver : class, IDependencyResolver
            where TActivator : class, IHttpControllerActivator
        {
            typeof(TResolver).VerifyIsClass();
            typeof(TActivator).VerifyIsClass();

            // We can register the types, and ILifetimeScope is provided by default.
            builder.RegisterType<TResolver>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<TActivator>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return builder;
        }

        /// <summary>
        /// Registers services with the <paramref name="builder"/>, including
        /// <typeparamref name="TResolver"/> and <typeparamref name="TActivator"/>.
        /// </summary>
        /// <typeparam name="TResolver"></typeparam>
        /// <typeparam name="TActivator"></typeparam>
        /// <typeparam name="TExceptionLogger"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ContainerBuilder RegisterApiServices<TResolver, TActivator, TExceptionLogger>(
            this ContainerBuilder builder)
            where TResolver : class, IDependencyResolver
            where TActivator : class, IHttpControllerActivator
            where TExceptionLogger : class, IExceptionLogger
        {
            builder.RegisterApiServices<TResolver, TActivator>();

            typeof(TExceptionLogger).VerifyIsClass();

            builder.RegisterType<TExceptionLogger>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            return builder;
        }

        /// <summary>
        /// Enables runtime interception using <see cref="HttpActionMeasurementProvider"/> by
        /// default.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        /// <see cref="EnableApiMeasurements{TInterface,TService,TOptions,TProvider}"/>
        public static ContainerBuilder EnableApiMeasurements<TInterface, TService>(
            this ContainerBuilder builder
            , Func<InstrumentationDiscoveryOptions> createOptions = null)
            where TInterface : class, IHttpActionInstrumentationDiscoveryService
            where TService : class, TInterface
        {
            return builder.EnableApiMeasurements<TInterface, TService
                , InstrumentationDiscoveryOptions
                , HttpActionMeasurementProvider>(createOptions);
        }

        /// <summary>
        /// Enables runtime interception using <see cref="HttpActionMeasurementProvider"/> by
        /// default.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        /// <see cref="EnableApiMeasurements{TInterface,TService,TOptions,TProvider}"/>
        public static ContainerBuilder EnableApiMeasurements<TInterface, TService, TOptions>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null)
            where TInterface : class, IHttpActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return builder.EnableApiMeasurements<TInterface, TService, TOptions
                , HttpActionMeasurementProvider>(createOptions);
        }

        // TODO: TBD: requiring IHttpActionInstrumentationDiscoveryService means that we will need to reference that as a package as well...
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
        /// <returns></returns>
        public static ContainerBuilder EnableApiMeasurements<TInterface, TService, TOptions, TProvider>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null)
            where TInterface : class, IHttpActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
            where TProvider : class, ITwoStageMeasurementProvider
        {
            {
                createOptions = createOptions ?? CreateDefaultDiscoveryOptions<TOptions>;

                builder.Register(context => createOptions())
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            // TODO: TBD: will need to be careful with the lifestyle here... or the capture/usage of it in the attribute...
            builder.RegisterType<TProvider>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            {
                typeof(TService).VerifyIsClass();
                typeof(TInterface).VerifyIsInterface();

                builder.RegisterType<TService>()
                    .AsImplementedInterfaces().InstancePerLifetimeScope();
            }

            return builder;
        }
    }
}
