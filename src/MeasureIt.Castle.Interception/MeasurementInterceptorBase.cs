namespace MeasureIt.Castle.Interception
{
    using Measurement;
    using global::Castle.DynamicProxy;

    public abstract class MeasurementInterceptorBase<TProvider> : IMeasurementInterceptor
        where TProvider : class, IInterceptionMeasurementProvider
    {
        protected TProvider Provider { get; private set; }

        protected MeasurementInterceptorBase(TProvider provider)
        {
            Provider = provider;
        }

        public abstract void Intercept(IInvocation invocation);
    }
}
