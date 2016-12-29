using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using global::Castle.Windsor;

    /// <summary>
    /// Controller activator for use with Castle Windsor.
    /// </summary>
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

        private IWindsorContainer Container { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public WindsorHttpControllerActivator(IWindsorContainer container)
        {
            Container = container;
        }

        public virtual IHttpController Create(HttpRequestMessage request,
            HttpControllerDescriptor ctrlDescriptor, Type ctrlType)
        {
            var ctrl = (IHttpController) Container.Resolve(ctrlType);

            request.RegisterForDispose(new ControllerReleaseResource(Container, ctrl));

            return ctrl;
        }
    }
}
