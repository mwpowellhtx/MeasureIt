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

        public abstract TContext GetMeasurementContext(Type targetType, MethodInfo method);
    }
}
