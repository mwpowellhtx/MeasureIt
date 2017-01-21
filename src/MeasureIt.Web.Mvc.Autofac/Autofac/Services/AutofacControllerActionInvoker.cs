using System.Linq;
using System.Web.Mvc;

namespace MeasureIt.Web.Mvc.Autofac
{
    using global::Autofac;

    /// <summary>
    /// <see cref="ControllerActionInvoker"/> for use with <see cref="IContainer"/>.
    /// </summary>
    public class AutofacControllerActionInvoker : ControllerActionInvoker, IAutofacControllerActionInvoker
    {
        private ILifetimeScope Scope { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scope"><see cref="ILifetimeScope"/> is automatically registered by
        /// Autofac, so no need to register again.</param>
        public AutofacControllerActionInvoker(ILifetimeScope scope)
        {
            Scope = scope;
        }

        private void InjectFilter<T>(T obj)
        {
            Scope.InjectObject(obj);
        }

        /// <summary>
        /// Returns the <see cref="FilterInfo"/> corresponding to the
        /// <paramref name="controllerContext"/> or <paramref name="actionDescriptor"/>.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filterInfo = base.GetFilters(controllerContext, actionDescriptor);

            // We must treat each of the Filter types individually.
            filterInfo.AuthorizationFilters.ToList().ForEach(InjectFilter);
            filterInfo.ActionFilters.ToList().ForEach(InjectFilter);
            filterInfo.ResultFilters.ToList().ForEach(InjectFilter);
            filterInfo.ExceptionFilters.ToList().ForEach(InjectFilter);

            return filterInfo;
        }
    }
}
