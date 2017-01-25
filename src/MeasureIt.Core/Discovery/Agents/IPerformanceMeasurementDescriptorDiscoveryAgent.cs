using System;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceMeasurementDescriptorDiscoveryAgent
        : IDiscoveryAgent<IPerformanceMeasurementDescriptor>
    {
    }

    // ReSharper disable once UnusedTypeParameter
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAttribute"></typeparam>
    public interface IPerformanceMeasurementDescriptorDiscoveryAgent<TAttribute>
        : IPerformanceMeasurementDescriptorDiscoveryAgent
        where TAttribute : Attribute, IMeasurePerformanceAttributeBase
    {
    }
}
