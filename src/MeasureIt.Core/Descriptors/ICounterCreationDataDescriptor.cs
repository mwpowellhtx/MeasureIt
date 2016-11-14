using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICounterCreationDataDescriptor : IDescriptor
    {
        /// <summary>
        /// Gets or sets the parent Adapter.
        /// </summary>
        IPerformanceCounterAdapter Adapter { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the Help.
        /// </summary>
        string Help { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PerformanceCounterType"/>.
        /// </summary>
        PerformanceCounterType CounterType { get; set; }

        /// <summary>
        /// Returns the a <see cref="CounterCreationData"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        CounterCreationData GetCounterCreationData();
    }
}
