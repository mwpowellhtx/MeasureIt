using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MeasureIt.Web.Http.Castle.Windsor
{
    using Contexts;
    using Discovery;
    using global::Castle.MicroKernel.Registration;
    using global::Castle.Windsor;

    /// <summary>
    /// 
    /// </summary>
    public static class RegistrationExtensionMethods
    {
        /// <summary>
        /// Registers the Api Controllers with the <paramref name="container"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="config"></param>
        /// <param name="assy"></param>
        /// <param name="otherAssies"></param>
        /// <returns></returns>
        public static IWindsorContainer RegisterApiControllers(this IWindsorContainer container,
            HttpConfiguration config, Assembly assy, params Assembly[] otherAssies)
        {
            container.Register(
                Component.For<IHttpControllerActivator>()
                    .ImplementedBy<WindsorHttpControllerActivator>()
                );

            config.Services.Replace(typeof(IHttpControllerActivator),
                container.Resolve<IHttpControllerActivator>());

            return new[] {assy}.Concat(otherAssies)
                .Aggregate(container,
                    (g, x) => g.Register(Classes.FromAssembly(x)
                        .BasedOn<IHttpController>().LifestyleTransient())
                );
        }

        // TODO: TBD: requiring IHttpActionInstrumentationDiscoveryService means that we will need to reference that as a package as well...
        /// <summary>
        /// Enables runtime interception using <typeparamref name="TService"/> via
        /// <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="container"></param>
        /// <param name="optsCreated"></param>
        /// <returns></returns>
        public static IWindsorContainer EnableApiMeasurements<TInterface, TService, TProvider>(
            this IWindsorContainer container
            , Action<IInstrumentationDiscoveryOptions> optsCreated = null)
            where TInterface : class, IHttpActionInstrumentationDiscoveryService
            where TService : class, TInterface
            where TProvider : class, ITwoStageMeasurementProvider
        {
            optsCreated = optsCreated ?? delegate { };

            container.Register(

                Component.For<IInstrumentationDiscoveryOptions>()
                    .ImplementedBy<InstrumentationDiscoveryOptions>()
                    .LifestyleTransient().OnCreate(optsCreated)

                // TODO: TBD: will need to be careful with the lifestyle here... or the capture/usage of it in the attribute...
                , Component.For<ITwoStageMeasurementProvider>()
                    .ImplementedBy<TProvider>().LifestyleTransient()

                );

            {
                var interfaceType = typeof(TInterface);

                interfaceType.VerifyIsInterface();

                var interfaceReg = Component.For<TInterface>()
                    .ImplementedBy<TService>().LifestyleTransient()
                    .Forward<TInterface, IRuntimeInstrumentationDiscoveryService>()
                    .Forward<TInterface, IInstallerInstrumentationDiscoveryService>()
                    ;

                container.Register(interfaceReg);
            }

            return container;
        }
    }
}
