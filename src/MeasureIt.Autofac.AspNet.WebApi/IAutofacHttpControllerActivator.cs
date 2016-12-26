using System;
using System.Web.Http.Dispatcher;

// ReSharper disable once CheckNamespace

namespace MeasureIt.Autofac
{
    /// <summary>
    /// Autofac Http controller activator.
    /// </summary>
    [Obsolete] // TODO: TBD: for the moment marking this as obsolete; not sure it is really necessary after all...
    public interface IAutofacHttpControllerActivator : IHttpControllerActivator
    {
    }
}
