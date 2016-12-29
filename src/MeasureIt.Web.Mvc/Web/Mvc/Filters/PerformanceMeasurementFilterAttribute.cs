using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace MeasureIt.Web.Mvc.Filters
{
    using Contexts;
    using static StatefulStorageMode;

    /// <summary>
    /// Performance measurement filter attribute.
    /// </summary>
    public class PerformanceMeasurementFilterAttribute : MeasurementFilterAttributeBase
    {
        /// <summary>
        /// Gets or sets the DependencyResolver.
        /// </summary>
        /// <remarks>We do not have many great choices when it comes to relaying interfaces such
        /// as this. Instead we will need to extend an <see cref="ControllerActionInvoker"/> in
        /// order to visit the filters with the appropriate resolutions. The documented example
        /// makes use of StructureMap, but a similar thing should work for both Castle Windsor
        /// as well as for Autofac.</remarks>
        /// <see cref="!:http://lostechies.com/jimmybogard/2010/05/03/dependency-injection-in-asp-net-mvc-filters/" />
        internal IDependencyResolver DependencyResolver { get; set; }

        private IStatefulStorage _storage;

        private readonly StatefulStorageMode _storageMode;

        /* TODO: TBD: I believe this is the core functionality that needs to take place:
         * does not yet take into account flags or sample rates and so forth */

        /// <summary>
        /// "mvc-measurement-provider"
        /// </summary>
        private const string MeasurementProviderKey = "mvc-measurement-provider";

        /// <summary>
        /// "mvc-measurement-context"
        /// </summary>
        private const string MeasurementContextKey = "mvc-measurement-context";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public PerformanceMeasurementFilterAttribute(Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
            : base(categoryType, adapterType, otherAdapterTypes)
        {
            _storageMode = PerRequest;
        }

        private ITwoStageMeasurementProvider GetMeasurementProvider()
        {
            return _storage.GetOrAdd(MeasurementProviderKey
                , () => DependencyResolver.GetService<ITwoStageMeasurementProvider>()
                );
        }

        private void BeginMeasurementContext(ActionExecutingContext filterContext, ITwoStageMeasurementProvider provider)
        {
            try
            {
                /* TODO: TBD: thinking about how in the world to test it... will need to consider a readonly set
                 * of counters, as well as a writable set, in order to get proper measurement that diagnostics are
                 * indeed taking place... */

                Func<ITwoStageMeasurementContext> getContext = () =>
                {
                    var ctrlType = filterContext.Controller.GetType();

                    var asyncActionDescriptor = filterContext.ActionDescriptor as ReflectedAsyncActionDescriptor;

                    if (asyncActionDescriptor != null)
                        return provider.GetMeasurementContext(ctrlType, asyncActionDescriptor.MethodInfo);

                    var actionDescriptor = filterContext.ActionDescriptor as ReflectedActionDescriptor;

                    // ReSharper disable once PossibleNullReferenceException
                    return provider.GetMeasurementContext(ctrlType, actionDescriptor.MethodInfo);
                };

                var measurementContext = _storage.GetOrAdd(MeasurementContextKey, getContext);

                if (measurementContext.MayReturn()) return;

                // TODO: TBD: signal that the measurement started? i.e. via stateful storage...
                // Start the Measurement ONLY. Leave Disposal for the End.
                measurementContext.Start(() => { });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ThrowPublishErrors) throw;
            }
        }

        private void EndMeasurementContext(ActionExecutedContext filterContext)
        {
            /* TODO: TBD: the thought is along the right lines, I believe; but the boundary likely
             * needs to be OnResultExecuted... */

            try
            {
                /* TODO: TBD: connect Counter Adapters with Measurement descriptors and monitor
                 * for cases like "errors", i.e. executedContext.Exception... */

                // We cannot tell whether the value would be Null, but it could be.
                var measurementContext = _storage.Get<ITwoStageMeasurementContext>(MeasurementContextKey);

                // May Stop the current Context while affording an opportunity to Dispose of its resources.
                if (measurementContext.MayReturn()) return;

                // ReSharper disable once PossibleNullReferenceException
                using (var stoppedContext = measurementContext.Stop())
                {
                    // Remember to set any error information that may have been encountered during execution.
                    stoppedContext.SetError(filterContext.Exception);
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
                _storage.TryRemove(MeasurementProviderKey);
                _storage.TryRemove(MeasurementContextKey);
            }
        }

        /* TODO: TBD: there does not appear to be any asynchrony in the event handlers, per se; rather,
         * those bits appear to be factored a bit differently via the ActionExecutingContext concern...
         * apart from evaluating ActionDescriptor for ReflectedActionDescriptor versus ReflectedAsyncActionDescriptor. */

        /// <summary>
        /// Action executing event handler.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _storage = new StatefulStorage(filterContext.HttpContext, _storageMode);
            base.OnActionExecuting(filterContext);
            var provider = GetMeasurementProvider();
            BeginMeasurementContext(filterContext, provider);
        }

        /// <summary>
        /// Action executed event handler.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            EndMeasurementContext(filterContext);
        }

        /* TODO: TBD: methinks I would want to evaluate from beginning of action executing, to end
         * of result executed... perhaps even have some additional boundaries in mind (?)... */
    }
}
