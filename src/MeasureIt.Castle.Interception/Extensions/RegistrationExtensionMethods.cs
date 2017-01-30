using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt
{
    using Castle.Interception;
    using Castle.Interception.Measurement;
    using global::Castle.DynamicProxy;

    /// <summary>
    /// Provides some enumeration extension methods.
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        private static IEnumerable<IInterceptor> GetRequiredInterceptors(
            IInterceptionMeasurementProvider measurementProvider
            , params IInterceptor[] interceptors)
        {
            /* This really is built with Dependency Injection in mind. Ad-hoc could work, but a lot
             * of resources will need to be provided. Perhaps that is an acceptable tradeoff. */

            yield return interceptors.SingleOrDefault(i => i is MeasurementInterceptor)
                         ?? new MeasurementInterceptor(measurementProvider);

            foreach (var i in interceptors.Where(i => !(i is MeasurementInterceptor)))
            {
                yield return i;
            }
        }

        /// <summary>
        /// Returns a proxy of the <paramref name="obj"/> with measurement enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="measurementProvider">Assumes that a provider has already been prepared.</param>
        /// <param name="createGenerationOptions"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static T MeasureSingleInstance<T>(this T obj
            , IInterceptionMeasurementProvider measurementProvider
            , Func<ProxyGenerationOptions> createGenerationOptions = null
            , params IInterceptor[] interceptors)
            where T : class, new()
        {
            return obj.MeasureSingleInstance(createGenerationOptions,
                GetRequiredInterceptors(measurementProvider, interceptors).ToArray());
        }

        /// <summary>
        /// Returns a proxy of the <paramref name="obj"/> with measurement enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="createGenerationOptions"></param>
        /// <param name="interceptors"></param>
        /// <returns></returns>
        public static T MeasureSingleInstance<T>(this T obj
            , Func<ProxyGenerationOptions> createGenerationOptions = null
            , params IInterceptor[] interceptors)
            where T : class, new()
        {
            typeof(T).VerifyIsClass();

            createGenerationOptions = createGenerationOptions ?? (() => new ProxyGenerationOptions());

            var generator = new ProxyGenerator();

            var generationOptions = createGenerationOptions();

            var proxy = generator.CreateClassProxyWithTarget(obj, generationOptions, interceptors);

            return proxy;
        }

    }
}
