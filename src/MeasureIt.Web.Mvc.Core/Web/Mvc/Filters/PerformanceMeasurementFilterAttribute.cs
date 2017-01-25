using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace MeasureIt.Web.Mvc.Filters
{
    using Contexts;
    using Kingdom.Web.Mvc;
    using IMvcDependencyResolver = IDependencyResolver;
    using static Discovery.MeasurementBoundary;
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
        [Inject]
        public IMvcDependencyResolver DependencyResolver { get; set; }

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
        /// "action-descriptor-key"
        /// </summary>
        private const string ActionDescriptorKey = "action-descriptor-key";

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
            Debug.Assert(DependencyResolver != null);

            return _storage.GetOrAdd(MeasurementProviderKey
                , () => DependencyResolver.GetService<ITwoStageMeasurementProvider>()
                );
        }

        private class MeasurementContextInfo
        {
            internal object FilterContext { get; }

            internal IController Controller { get; }

            internal ActionDescriptor ActionDescriptor { get; set; }

            internal Exception Exception { get; }

            internal MeasurementContextInfo(ActionExecutingContext filterContext)
                : this(filterContext, filterContext.Controller, filterContext.ActionDescriptor)
            {
            }

            internal MeasurementContextInfo(ActionExecutedContext filterContext)
                : this(filterContext, filterContext.Controller, filterContext.ActionDescriptor, filterContext.Exception)
            {
            }

            internal MeasurementContextInfo(ResultExecutingContext filterContext)
                : this(filterContext, filterContext.Controller)
            {
            }

            internal MeasurementContextInfo(ResultExecutedContext filterContext)
                : this(filterContext, filterContext.Controller, filterContext.Exception)
            {
            }

            private MeasurementContextInfo(object filterContext, IController ctrl, Exception ex = null)
            {
                Controller = ctrl;
                FilterContext = filterContext;
                Exception = ex;
            }

            private MeasurementContextInfo(object filterContext, IController ctrl, ActionDescriptor actionDescriptor, Exception ex = null)
            {
                Controller = ctrl;
                FilterContext = filterContext;
                ActionDescriptor = actionDescriptor;
                Exception = ex;
            }
        }

        private void BeginMeasurementContext(MeasurementContextInfo contextInfo)
        {
            try
            {
                var provider = GetMeasurementProvider();

                var actionDescriptor = _storage.Get<ActionDescriptor>(ActionDescriptorKey);

                /* TODO: TBD: thinking about how in the world to test it... will need to consider a readonly set
                 * of counters, as well as a writable set, in order to get proper measurement that diagnostics are
                 * indeed taking place... */

                Func<ITwoStageMeasurementContext> createContext = () =>
                {
                    var ctrlType = contextInfo.Controller.GetType();

                    return provider.GetMeasurementContext(ctrlType, actionDescriptor is ReflectedAsyncActionDescriptor
                        ? ((ReflectedAsyncActionDescriptor) actionDescriptor).MethodInfo
                        : ((ReflectedActionDescriptor) actionDescriptor).MethodInfo);
                };

                var measurementContext = _storage.GetOrAdd(MeasurementContextKey, createContext);

                if (measurementContext.MayReturn()) return;

                // TODO: TBD: signal that the measurement started? i.e. via stateful storage...
                // Start the Measurement ONLY. Leave Disposal for the End.
                measurementContext.Start(() => { });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                Descriptor.SetError(ex);
                if (ThrowPublishErrors) throw;
            }
        }

        private void EndMeasurementContext(MeasurementContextInfo contextInfo)
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
                    stoppedContext.SetError(contextInfo.Exception);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                Descriptor.SetError(ex);
                if (ThrowPublishErrors) throw;
            }
            finally
            {
                // Try to remove the Properties.
                _storage.TryRemove(MeasurementProviderKey);
                _storage.TryRemove(MeasurementContextKey);
                _storage.TryRemove(ActionDescriptorKey);
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

            /* In this case we do want to add, basically, the Descriptor. This is so that
             * the Descriptor is available later on, regardless of the boundary. */

            _storage.GetOrAdd(ActionDescriptorKey, () => filterContext.ActionDescriptor);

            if (StartBoundary == BeginAction)
            {
                BeginMeasurementContext(new MeasurementContextInfo(filterContext));
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Action executed event handler.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var lazyInfo = new Lazy<MeasurementContextInfo>(
                () => new MeasurementContextInfo(filterContext)
                );

            if (StartBoundary == EndAction)
            {
                BeginMeasurementContext(lazyInfo.Value);
            }

            base.OnActionExecuted(filterContext);

            if (StopBoundary == EndAction)
            {
                EndMeasurementContext(lazyInfo.Value);
            }
        }

        /* TODO: TBD: methinks I would want to evaluate from beginning of action executing, to end
         * of result executed... perhaps even have some additional boundaries in mind (?)... */

        /// <summary>
        /// Result executing event handler.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var lazyInfo = new Lazy<MeasurementContextInfo>(
                () =>
                {
                    var actionDescriptor = _storage.Get<ActionDescriptor>(ActionDescriptorKey);
                    return new MeasurementContextInfo(filterContext) {ActionDescriptor = actionDescriptor};
                });

            // I don't know why you would do this, but it's possible...
            if (StartBoundary == BeginResult)
            {
                BeginMeasurementContext(lazyInfo.Value);
            }

            base.OnResultExecuting(filterContext);

            if (StopBoundary == BeginResult)
            {
                EndMeasurementContext(lazyInfo.Value);
            }
        }

        /// <summary>
        /// Result executed event handler.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            if (StopBoundary == EndResult)
            {
                EndMeasurementContext(new MeasurementContextInfo(filterContext));
            }
        }
    }
}
