using System;
using System.Net.Http;
using System.Web.Http.Controllers;

// ReSharper disable once CheckNamespace

namespace MeasureIt.Autofac
{
    using global::Autofac;

    /// <summary>
    /// 
    /// </summary>
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
                if (_disposable == null) return;
                _disposable.Dispose();
            }
        }

        private readonly ILifetimeScope _scope;

        private AutofacHttpControllerActivator(ILifetimeScope scope)
        {
            _scope = scope;
        }

        internal static IAutofacHttpControllerActivator Create(ILifetimeScope scope)
        {
            return new AutofacHttpControllerActivator(scope);
        }

        public virtual IHttpController Create(HttpRequestMessage request,
            HttpControllerDescriptor ctrlDescriptor, Type ctrlType)
        {
            var ctrl = (IHttpController) _scope.Resolve(ctrlType);

            request.RegisterForDispose(new ControllerReleaseResource(ctrl));

            return ctrl;
        }
    }
}
