namespace MeasureIt.Castle.Interception
{
    using global::Castle.DynamicProxy;
    using Contexts;

    public abstract class MeasurementInterceptorBase<TMeasurementProvider> : IMeasurementInterceptor
        where TMeasurementProvider : IMeasurementProvider
    {
        protected TMeasurementProvider Provider { get; private set; }

        protected MeasurementInterceptorBase(TMeasurementProvider provider)
        {
            Provider = provider;
        }

        public abstract void Intercept(IInvocation invocation);
    }
}
