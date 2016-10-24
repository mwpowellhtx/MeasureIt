using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICounterCreationDataDescriptor : IDescriptor
    {
        /// <summary>
        /// Gets or sets the AdapterDescriptor.
        /// </summary>
        IPerformanceCounterAdapterDescriptor AdapterDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the CounterName.
        /// </summary>
        string CounterName { get; set; }

        /// <summary>
        /// Gets or sets the Help.
        /// </summary>
        string Help { get; set; }

        /// <summary>
        /// Gets or sets the InstanceName.
        /// </summary>
        string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets whether ReadOnly.
        /// </summary>
        bool? ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PerformanceCounterType"/>.
        /// </summary>
        PerformanceCounterType CounterType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PerformanceCounterInstanceLifetime"/>.
        /// </summary>
        PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// Returns the a <see cref="CounterCreationData"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        CounterCreationData GetCounterCreationData();
    }
}
