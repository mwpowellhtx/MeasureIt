using System;

namespace MeasureIt.Web.Mvc.Discovery.Agents
{
    using Filters;
    using MeasureIt.Discovery.Agents;

    /// <summary>
    /// Measurement filter discovery agent.
    /// </summary>
    /// <typeparamref name="TAttribute"/>
    public interface IMeasurementFilterDiscoveryAgent<TAttribute>
        : IPerformanceMeasurementDescriptorDiscoveryAgent<TAttribute>
        where TAttribute : Attribute, IMeasurePerformanceAttribute
    {
    }

    /// <summary>
    /// Measurement filter discovery agent.
    /// </summary>
    public interface IMeasurementFilterDiscoveryAgent
        : IMeasurementFilterDiscoveryAgent<
            PerformanceMeasurementFilterAttribute>
    {
    }
}
