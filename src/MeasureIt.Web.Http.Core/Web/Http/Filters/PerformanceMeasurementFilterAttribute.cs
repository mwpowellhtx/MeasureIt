using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MeasureIt.Web.Http.Filters
{
    using Contexts;

    /// <summary>
    /// This Attribute serves a dual purpose. First is to configure measurement conditions for the
    /// action on which it is a decoration. Second is as a focal point for interception, similar
    /// to Castle DynamicProxy IInterceptor.
    /// </summary>
    public class PerformanceMeasurementFilterAttribute : MeasurementFilterAttributeBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementFilterAttribute(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : base(categoryType, adapterType, otherAdapterTypes)
        {
        }

        /* TODO: TBD: I believe this is the core functionality that needs to take place:
         * does not yet take into account flags or sample rates and so forth */

        /// <summary>
        /// "measurement-provider"
        /// </summary>
        private const string MeasurementProviderKey = "measurement-provider";

        /// <summary>
        /// "measurement-context"
        /// </summary>
        private const string MeasurementContextKey = "measurement-context";

        private static ITwoStageMeasurementProvider GetMeasurementProvider(HttpActionContext actionContext)
        {
            var config = actionContext.RequestContext.Configuration;
            var properties = actionContext.Request.Properties;

            var provider = (ITwoStageMeasurementProvider)
                (properties.ContainsKey(MeasurementProviderKey)
                    ? properties[MeasurementProviderKey]
                    : properties[MeasurementProviderKey]
                        = config.DependencyResolver.GetService<ITwoStageMeasurementProvider>());

            // Here we separate the concern of the Web Http framework from that of your favorite DI container.
            return provider;
        }

        private void BeginMeasurementContext(HttpActionContext actionContext, ITwoStageMeasurementProvider provider)
        {
            // TODO: TBD: thinking about how in the world to test it... will need to consider a readonly set of counters, as well as a writable set, in order to get proper measurement that diagnostics are indeed taking place...
            try
            {
                const string key = MeasurementContextKey;

                var properties = actionContext.Request.Properties;

                Func<ITwoStageMeasurementContext> getContext = () =>
                {
                    var ctrlType = actionContext.ControllerContext.ControllerDescriptor.ControllerType;
                    var actionDescriptor = actionContext.ActionDescriptor as ReflectedHttpActionDescriptor;
                    // ReSharper disable once PossibleNullReferenceException
                    return provider.GetMeasurementContext(ctrlType, actionDescriptor.MethodInfo);
                };

                var measurementContext = properties.ContainsKey(key)
                    ? (ITwoStageMeasurementContext) properties[key]
                    : (ITwoStageMeasurementContext) (properties[key] = getContext())
                    ;

                if (measurementContext.MayReturn()) return;

                // Start the Measurement ONLY. Leave Disposal for the End.
                measurementContext.Start(actionContext.Response);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ThrowPublishErrors) throw;
            }
        }

        private void EndMeasurementContext(HttpActionExecutedContext executedContext)
        {
            var properties = executedContext.Request.Properties;

            // TODO: TBD: connect Counter Adapters with Measurement descriptors and monitor for cases like "errors", i.e. executedContext.Exception...

            try
            {
                const string key = MeasurementContextKey;

                var measurementContext = (ITwoStageMeasurementContext)
                    (properties.ContainsKey(key)
                        ? properties[key]
                        : null);

                // May Stop the current Context while affording an opportunity to Dispose of its resources.
                if (measurementContext.MayReturn()) return;

                // ReSharper disable once PossibleNullReferenceException
                using (var stoppedContext = measurementContext.Stop())
                {
                    // Remember to set any error information that may have been encountered during execution.
                    stoppedContext.SetError(executedContext.Exception);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ThrowPublishErrors) throw;
            }
            finally
            {
                // Try to remove the Properties.
                properties.TryRemove(MeasurementProviderKey);
                properties.TryRemove(MeasurementContextKey);
            }
        }

        /// <summary>
        /// Action executed event handler.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var provider = GetMeasurementProvider(actionContext);
            BeginMeasurementContext(actionContext, provider);
        }

        /// <summary>
        /// Asynchronously handles the Action executing event.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            // TODO: TBD: ditto, except async...
            return base.OnActionExecutingAsync(actionContext, cancellationToken)
                .ContinueWith(x =>
                {
                    var provider = GetMeasurementProvider(actionContext);
                    BeginMeasurementContext(actionContext, provider);
                }, cancellationToken);
        }

        /// <summary>
        /// Action executed event handler.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            EndMeasurementContext(actionExecutedContext);
        }

        /// <summary>
        /// Asynchronously handles the Action executed event.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            // TODO: TBD: the only danger I can foresee here is that the tasks might not be awaited until a later time than expected...

            /* Don't know if the base class does anything, but let's let it happen and continue
             * with what we want to do next. */

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken)
                .ContinueWith(x => EndMeasurementContext(actionExecutedContext), cancellationToken);
        }
    }
}
