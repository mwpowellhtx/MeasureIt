using System;

namespace MeasureIt.Discovery.Agents
{
    using Web.Http.Filters;

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
