using System;
using MeasureIt.Web.Http.Filters;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparamref name="TAttribute"/>
    public interface IMeasurementFilterDiscoveryAgent<TAttribute>
        : IPerformanceMeasurementDescriptorDiscoveryAgent<TAttribute>
        where TAttribute : Attribute, IMeasurePerformanceAttribute
    {
    }

    public interface IMeasurementFilterDiscoveryAgent
        : IMeasurementFilterDiscoveryAgent<
            PerformanceMeasurementFilterAttribute>
    {
    }
}
