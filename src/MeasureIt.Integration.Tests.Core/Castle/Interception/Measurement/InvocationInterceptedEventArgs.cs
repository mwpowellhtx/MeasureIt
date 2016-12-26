using System;

namespace MeasureIt.Castle.Interception.Measurement
{
    using global::Castle.DynamicProxy;

    public class InvocationInterceptedEventArgs : EventArgs
    {
        internal IInvocation Invocation { get; private set; }

        internal InvocationInterceptedEventArgs(IInvocation invocation)
        {
            Invocation = invocation;
        }
    }
}
