using System.Collections.Generic;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// InstrumentationDiscoveryService interaface.
    /// </summary>
    public interface IInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IPerformanceCounterAdapterDescriptor> CounterAdapterDescriptors { get; }

        /// <summary>
        /// Discovers the Performance Monitoring Instrumentation from the Assemblies.
        /// </summary>
        void Discover();

        // TODO: TBD: may want to setup a "proper" enum value
        /// <summary>
        /// 
        /// </summary>
        bool IsPending { get; }
    }
}
