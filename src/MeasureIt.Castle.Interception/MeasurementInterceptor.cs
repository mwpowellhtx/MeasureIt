using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Castle.Interception
{
    using global::Castle.DynamicProxy;
    using Measurement;

    public class MeasurementInterceptor : MeasurementInterceptorBase
    {
        public MeasurementInterceptor(IMeasurementProvider measurementProvider)
            : base(measurementProvider)
        {
        }

        public override void Intercept(IInvocation invocation)
        {
            var context = Provider.GetMeasurementContext(invocation.TargetType, invocation.Method);

            if (context == null)
            {
                invocation.Proceed();
                return;
            }

            using (context)
            {
                try
                {
                    if (context.Descriptor.MayProceedUnabated)
                    {
                        invocation.Proceed();
                        return;
                    }

                    // TODO: proceed with measurements...

                    var returnType = invocation.Method.ReturnType;

                    // TODO: TBD: what does not being Void have to do with anything? if indeed we are testing FOR being a Task, generic or not
                    // TODO: TBD: also, stop and look at Task itself: base class for Task<>
                    if (returnType != typeof(void)
                        && typeof(Task).IsAssignableFrom(returnType))
                    {
#pragma warning disable 1998
                        context.MeasureAsync(async () => invocation.Proceed()).Wait();
#pragma warning restore 1998
                    }
                    else
                    {
                        context.Measure(invocation.Proceed);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    if (context.Descriptor.ThrowPublishErrors) throw;
                }
            }
        }
    }
}
