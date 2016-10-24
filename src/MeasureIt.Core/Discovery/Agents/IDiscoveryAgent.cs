using System.Collections.Generic;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDiscoveryAgent<out T> : IEnumerable<T>
    {
    }
}
