using System;

namespace MeasureIt.Castle.Interception.Measurement
{
    using global::Castle.DynamicProxy;

    public class MeasurementInterceptorFixture : MeasurementInterceptor, IMeasurementInterceptorFixture
    {
        public static event EventHandler<InvocationInterceptedEventArgs> Intercepted;

        public static event EventHandler<InvocationInterceptedEventArgs> Measuring;

        public static event EventHandler<InvocationInterceptedEventArgs> Measured;

        private static void RaiseIntercepted(object sender, InvocationInterceptedEventArgs e)
        {
            if (Intercepted == null) return;
            Intercepted(sender, e);
        }

        private static void RaiseMeasuring(object sender, InvocationInterceptedEventArgs e)
        {
            if (Measuring == null) return;
            Measuring(sender, e);
        }

        private static void RaiseMeasured(object sender, InvocationInterceptedEventArgs e)
        {
            if (Measured == null) return;
            Measured(sender, e);
        }

        public MeasurementInterceptorFixture(IInterceptionMeasurementProvider measurementProvider)
            : base(measurementProvider)
        {
        }

        protected override void OnMeasuring(IInvocation invocation)
        {
            base.OnMeasuring(invocation);

            RaiseMeasuring(this, new InvocationInterceptedEventArgs(invocation));
        }

        protected override void OnMeasured(IInvocation invocation)
        {
            base.OnMeasured(invocation);

            RaiseMeasured(this, new InvocationInterceptedEventArgs(invocation));
        }

        public override void Intercept(IInvocation invocation)
        {
            RaiseIntercepted(this, new InvocationInterceptedEventArgs(invocation));

            base.Intercept(invocation);
        }
    }
}
