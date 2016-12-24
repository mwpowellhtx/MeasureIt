using System;
using System.Reflection;

namespace MeasureIt.Contexts
{
    using Discovery;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MeasurementProviderBase<TContext> : IMeasurementProvider<TContext>
        where TContext : class, IMeasurementContext
    {
        /// <summary>
        /// Gets the Options.
        /// </summary>
        protected IInstrumentationDiscoveryOptions Options { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        protected MeasurementProviderBase(IInstrumentationDiscoveryOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Returns the <typeparamref name="TContext"/> corresponding with the Provider,
        /// <paramref name="targetType"/> and <paramref name="method"/>.
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public abstract TContext GetMeasurementContext(Type targetType, MethodInfo method);
    }
}
