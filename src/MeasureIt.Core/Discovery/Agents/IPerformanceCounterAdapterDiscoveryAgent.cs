namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// Agent for purposes of discovering Performance Counter Adapters.
    /// </summary>
    public interface IPerformanceCounterAdapterDiscoveryAgent
        : IDiscoveryAgent<IPerformanceCounterAdapter>
    {
    }
}
