using System;
using System.Net.Http;
using System.Web.Http.Controllers;

// ReSharper disable once CheckNamespace

namespace MeasureIt.Web.Http.Autofac
{
    using global::Autofac;

    /// <summary>
    /// Autofac Http controller activator.
    /// </summary>
    [Obsolete] // TODO: TBD: ditto interface; not sure it is necessary after all...
    public class AutofacHttpControllerActivator : IAutofacHttpControllerActivator
    {
        private class ControllerReleaseResource : IDisposable
        {
            private readonly IDisposable _disposable;

            internal ControllerReleaseResource(IHttpController ctrl)
            {
                _disposable = ctrl as IDisposable;
            }

            public void Dispose()
            {
                _disposable?.Dispose();
            }
        }

        private readonly ILifetimeScope _scope;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scope"></param>
        public AutofacHttpControllerActivator(ILifetimeScope scope)
        {
            _scope = scope;
        }

        /// <summary>
        /// Returns a Created <see cref="IHttpController"/> corresponding with the
        /// <paramref name="request"/> and <paramref name="ctrlType"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ctrlDescriptor"></param>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        public virtual IHttpController Create(HttpRequestMessage request,
            HttpControllerDescriptor ctrlDescriptor, Type ctrlType)
        {
            var ctrl = (IHttpController) _scope.Resolve(ctrlType);

            request.RegisterForDispose(new ControllerReleaseResource(ctrl));

            return ctrl;
        }
    }
}
