﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MeasureIt.Castle.Interception
{
    using global::Castle.DynamicProxy;
    using Measurement;

    /// <summary>
    /// Provides Interceptor services for Measurement purposes.
    /// </summary>
    public class MeasurementInterceptor : MeasurementInterceptorBase<IInterceptionMeasurementProvider>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="measurementProvider"></param>
        public MeasurementInterceptor(IInterceptionMeasurementProvider measurementProvider)
            : base(measurementProvider)
        {
        }

        /// <summary>
        /// Measuring event handler.
        /// </summary>
        /// <param name="invocation"></param>
        protected virtual void OnMeasuring(IInvocation invocation)
        {
        }

        /// <summary>
        /// Measured event handler.
        /// </summary>
        /// <param name="invocation"></param>
        protected virtual void OnMeasured(IInvocation invocation)
        {
        }

        /// <summary>
        /// Intercept event handler.
        /// </summary>
        /// <param name="invocation"></param>
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
                        OnMeasuring(invocation);

                        i.Proceed();

                        OnMeasured(invocation);

                        return;
                    }

                    OnMeasuring(invocation);

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

                    OnMeasured(invocation);
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
