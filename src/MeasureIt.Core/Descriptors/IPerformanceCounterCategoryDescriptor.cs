using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Category Descriptor captures the elements of the metadata described through the
    /// <see cref="PerformanceCounterCategoryAttribute"/>. Unlike the other
    /// <see cref="IDescriptor"/> types, this Descriptor type is along for the ride when a
    /// <see cref="IPerformanceCounterCategoryAttribute"/> is created as a less active passenger,
    /// and should not be confused with <see cref="IPerformanceCounterCategoryAdapter"/>.
    /// </summary>
    public interface IPerformanceCounterCategoryDescriptor : IDescriptor
    {
        /// <summary>
        /// Gets or sets the Type.
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the Help.
        /// </summary>
        string Help { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PerformanceCounterCategoryType"/>.
        /// </summary>
        PerformanceCounterCategoryType CategoryType { get; set; }

        // TODO: TBD: we may need/want to dive deeper here and actually uncover the IMeasurePerformanceDescriptor's corresponding with the category
        // TODO: TBD: in this sense, we may then want/need the category adapter System.Type

        /// <summary>
        /// Gets or sets the set of <see cref="ICounterCreationDataDescriptor"/> items.
        /// </summary>
        IEnumerable<ICounterCreationDataDescriptor> CreationDataDescriptors { get; set; }

        /// <summary>
        /// Returns the <see cref="CounterCreationDataCollection"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        CounterCreationDataCollection GetCounterCreationDataCollection();
    }
}
