using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace MeasureIt.Web.Http
{
    /// <summary>
    /// Provides a Trace capability during <see cref="Exception"/> logging.
    /// </summary>
    public class TraceExceptionLogger : ExceptionLogger
    {
        private static void Log(Exception ex)
        {
            Trace.TraceError(ex.ToString());
            throw ex;
        }

        /// <summary>
        /// Logs the <paramref name="context"/>.
        /// </summary>
        /// <param name="context"></param>
        public override void Log(ExceptionLoggerContext context)
        {
            Log(context.Exception);

            base.Log(context);
        }

        /// <summary>
        /// Logs the <paramref name="context"/> asynchronously.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            // ReSharper disable once ImplicitlyCapturedClosure
            return Task.Run(() => Log(context.Exception), cancellationToken)
                .ContinueWith(delegate
                {
                    base.LogAsync(context, cancellationToken);
                }, cancellationToken);
        }
    }
}
