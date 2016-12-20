using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace MeasureIt.Castle.Windsor
{
    using global::Castle.Windsor;

    public class WindsorHttpControllerActivator : IWindsorHttpControllerActivator
    {
        private class ControllerReleaseResource : IDisposable
        {
            private readonly IHttpController _ctrl;

            private readonly IWindsorContainer _container;

            internal ControllerReleaseResource(IWindsorContainer container, IHttpController ctrl)
            {
                _ctrl = ctrl;
                _container = container;
            }

            public void Dispose()
            {
                _container.Kernel.ReleaseComponent(_ctrl);
            }
        }

        private readonly IWindsorContainer _container;

        public WindsorHttpControllerActivator(IWindsorContainer container)
        {
            _container = container;
        }

        public virtual IHttpController Create(HttpRequestMessage request,
            HttpControllerDescriptor ctrlDescriptor, Type ctrlType)
        {
            var ctrl = (IHttpController) _container.Resolve(ctrlType);

            request.RegisterForDispose(new ControllerReleaseResource(_container, ctrl));

            return ctrl;
        }
    }
}
