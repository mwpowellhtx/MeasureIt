namespace MeasureIt.Castle.Interception
{
    using global::Castle.DynamicProxy;
    using Measurement;

    public abstract class MeasurementInterceptorBase : IMeasurementInterceptor
    {
        protected IMeasurementProvider Provider { get; private set; }

        protected MeasurementInterceptorBase(IMeasurementProvider provider)
        {
            Provider = provider;
        }

        public abstract void Intercept(IInvocation invocation);
    }
}
