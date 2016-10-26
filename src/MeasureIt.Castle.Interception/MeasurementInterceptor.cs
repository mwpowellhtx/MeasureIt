using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Castle.Interception
{
    using global::Castle.DynamicProxy;
    using Measurement;

    public class MeasurementInterceptor : MeasurementInterceptorBase<IInterceptionMeasurementProvider>
    {
        public MeasurementInterceptor(IInterceptionMeasurementProvider measurementProvider)
            : base(measurementProvider)
        {
        }

        public override void Intercept(IInvocation invocation)
        {
            var i = invocation;

            var context = Provider.GetMeasurementContext(i.TargetType, i.Method);

            if (context == null)
            {
                i.Proceed();
                return;
            }

            using (context)
            {
                try
                {
                    if (context.Descriptor.MayProceedUnabated)
                    {
                        i.Proceed();
                        return;
                    }

                    // TODO: proceed with measurements...
                    var returnType = i.Method.ReturnType;

                    // TODO: TBD: what does not being Void have to do with anything? if indeed we are testing FOR being a Task, generic or not
                    // TODO: TBD: also, stop and look at Task itself: base class for Task<>
                    if (returnType != typeof(void)
                        && typeof(Task).IsAssignableFrom(returnType))
                    {
#pragma warning disable 1998
                        context.MeasureAsync(async () => i.Proceed()).Wait();
#pragma warning restore 1998
                    }
                    else
                    {
                        context.Measure(i.Proceed);
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
