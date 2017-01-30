namespace MeasureIt.Castle.Interception
{
    using Measurement;
    using global::Castle.DynamicProxy;

    /// <summary>
    /// Interceptor base class for Measurement purposes.
    /// </summary>
    /// <typeparam name="TProvider"></typeparam>
    public abstract class MeasurementInterceptorBase<TProvider> : IMeasurementInterceptor
        where TProvider : class, IInterceptionMeasurementProvider
    {
        /// <summary>
        /// Gets the Provider associated with the Interceptor.
        /// </summary>
        protected TProvider Provider { get; private set; }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="provider"></param>
        protected MeasurementInterceptorBase(TProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Intercept event handler.
        /// </summary>
        /// <param name="invocation"></param>
        public abstract void Intercept(IInvocation invocation);
    }
}
