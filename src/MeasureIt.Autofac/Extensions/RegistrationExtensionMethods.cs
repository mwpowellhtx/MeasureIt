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
    using static Discovery.InstrumentationDiscoveryOptions;
    using static Castle.DynamicProxy.AutofacEnableInterceptionOption;

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
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        public static ContainerBuilder EnableMeasurements<TInterface, TService>(
            this ContainerBuilder builder
            , Func<InstrumentationDiscoveryOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
        {
            return builder.EnableMeasurements<TInterface, TService
                , InstrumentationDiscoveryOptions
                , MeasurementInterceptor>(createOptions);
        }

        /// <summary>
        /// Enables Measurements given the configuration represented by
        /// <typeparamref name="TInterface"/> and <typeparamref name="TService"/>.
        /// <see cref="MeasurementInterceptor"/> is assumed to be the
        /// <see cref="IMeasurementInterceptor"/> implementation.
        /// </summary>
        /// <typeparam name="TInterface">This is the interface implemented by <typeparamref name="TService"/>.</typeparam>
        /// <typeparam name="TService">Service represents a concrete implementation of <typeparamref name="TInterface"/>.</typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        /// <see cref="EnableMeasurements{TInterface,TService,TInterceptor,TOptions}"/>
        public static ContainerBuilder EnableMeasurements<TInterface, TService, TOptions>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return builder.EnableMeasurements<TInterface, TService, TOptions
                , MeasurementInterceptor>(createOptions);
        }

        /// <summary>
        /// Enables Measurements given the configuration represented by
        /// <typeparamref name="TInterface"/>, <typeparamref name="TService"/>,
        /// and <typeparamref name="TInterceptor"/>.
        /// </summary>
        /// <typeparam name="TInterface">This is the interface implemented by <typeparamref name="TService"/>.</typeparam>
        /// <typeparam name="TService">Service represents a concrete implementation of <typeparamref name="TInterface"/>.</typeparam>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TInterceptor">This is the concrete implementation of <see cref="IMeasurementInterceptor"/>.
        /// This is usually <see cref="MeasurementInterceptor"/>, but can be your own implementation.</typeparam>
        /// <param name="builder"></param>
        /// <param name="createOptions"></param>
        /// <returns></returns>
        /// <see cref="EnableMeasurements{TInterface,TService,TOptions}"/>
        public static ContainerBuilder EnableMeasurements<TInterface, TService, TOptions, TInterceptor>(
            this ContainerBuilder builder
            , Func<TOptions> createOptions = null)
            where TInterface : class, IRuntimeInstrumentationDiscoveryService
            where TService : class, TInterface
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
            where TInterceptor : class, IMeasurementInterceptor
        {
            {
                typeof(TOptions).VerifyIsClass();

                createOptions = createOptions ?? CreateDefaultDiscoveryOptions<TOptions>;

                builder.Register(context => createOptions())
                    .AsImplementedInterfaces()
                    .SingleInstance();
            }

            {
                builder.RegisterType<InterceptionMeasurementProvider>()
                    .AsImplementedInterfaces()
                    .InstancePerDependency();

                builder.RegisterType<TInterceptor>()
                    .AsImplementedInterfaces()
                    .InstancePerDependency();
            }

            {
                typeof(TInterface).VerifyIsInterface();
                typeof(TService).VerifyIsClass();

                builder.RegisterType<TService>()
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            return builder;
        }

        /// <summary>
        /// Returns the values defined by the <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<T> GetEnumValues<T>()
        {
            var type = typeof(T);

            if (!type.IsEnum)
            {
                var message = $"Type {typeof(T).FullName} is not an enum type.";
                throw new InvalidOperationException(message);
            }

            foreach (T value in Enum.GetValues(type)) yield return value;
        }

        /// <summary>
        /// Returns a Created Instance of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T CreateDefaultInstance<T>()
            where T : class, new()
        {
            return new T();
        }

        /// <summary>
        /// Registers the <typeparamref name="TService"/> with interception support by the
        /// <typeparamref name="TInterceptor"/>.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="builder"></param>
        /// <param name="createGeneratorOptions">Enables <see cref="Class"/> or
        /// <see cref="Interface"/> interception, depending on the
        /// <see cref="AutofacProxyGenerationOptions"/> that were provided.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when options are insufficient to proceed.</exception>
        public static ContainerBuilder EnableMeasurementInterception<TInterface, TService, TInterceptor>(
            this ContainerBuilder builder
            , Func<AutofacProxyGenerationOptions> createGeneratorOptions = null)
            where TInterface : class
            where TService : class, TInterface
            where TInterceptor : class, IMeasurementInterceptor
        {
            createGeneratorOptions = createGeneratorOptions ?? CreateDefaultInstance<AutofacProxyGenerationOptions>;

            var generatorOptions = createGeneratorOptions();

            // I'm not sure how this "simplifies" the LINQ, but we'll run with it anyway.
            if (GetEnumValues<AutofacEnableInterceptionOption>()
                .All(x => generatorOptions.EnableInterception.TryContains(x)))
            {
                var optionsType = typeof(AutofacProxyGenerationOptions);

                var property = optionsType.GetProperty(nameof(AutofacProxyGenerationOptions.EnableInterception));

                var message = $"Expected an '{typeof(AutofacEnableInterceptionOption).FullName}'"
                              + $" value for '{optionsType.FullName}.{property.Name}'.";

                throw new InvalidOperationException(message);
            }

            typeof(TInterface).VerifyIsInterface();
            typeof(TService).VerifyIsClass();

            var regBuilder = builder
                .RegisterType<TService>()
                .AsImplementedInterfaces();

            // TODO: TBD: whether to capture enable xyz interceptors (Type[] additionalInterfaces) args via options
            if (generatorOptions.EnableInterception.TryContains(Class))
            {
                regBuilder.EnableClassInterceptors(generatorOptions);
            }

            if (generatorOptions.EnableInterception.TryContains(Interface))
            {
                regBuilder.EnableInterfaceInterceptors(generatorOptions);
            }

            var interceptorType = typeof(TInterceptor);

            interceptorType.VerifyIsClass();

            regBuilder.InterceptedBy(interceptorType);

            return builder;
        }

        /// <summary>
        /// Measure the <paramref name="obj"/> Instance providing for <paramref name="container"/>
        /// and <paramref name="createGeneratorOptions"/>. In this case all we need are
        /// <see cref="ProxyGenerationOptions"/> options. No need to get specific with
        /// <see cref="AutofacProxyGenerationOptions"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TInterceptor"></typeparam>
        /// <param name="container"></param>
        /// <param name="obj"></param>
        /// <param name="createGeneratorOptions"></param>
        /// <returns></returns>
        public static T AsMeasuredInstance<T, TInterceptor>(this IContainer container, T obj
            , Func<ProxyGenerationOptions> createGeneratorOptions = null)
            where T : class
            where TInterceptor : class, IMeasurementInterceptor
        {
            // ReSharper disable once ConvertToLambdaExpression
            createGeneratorOptions = createGeneratorOptions ?? CreateDefaultInstance<ProxyGenerationOptions>;

            var generatorOptions = createGeneratorOptions();

            var generator = new ProxyGenerator();

            var interceptors = container.Resolve<IEnumerable<TInterceptor>>().ToArray<IInterceptor>();

            var proxy = generator.CreateClassProxyWithTarget(obj, generatorOptions, interceptors);

            return proxy;
        }
    }
}
