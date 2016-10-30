using System;
using System.Collections.Generic;
using System.Diagnostics;
using MeasureIt.Collections.Generic;

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
        /// Gets the Measurements.
        /// </summary>
        IList<IPerformanceMeasurementDescriptor> Measurements { get; }

        /// <summary>
        /// Gets or sets the set of <see cref="ICounterCreationDataDescriptor"/> items.
        /// </summary>
        IEnumerable<ICounterCreationDataDescriptor> CreationDataDescriptors { get; set; }

        /// <summary>
        /// Returns a created <see cref="PerformanceCounterCategory"/> corresponding with the
        /// <see cref="Measurements"/>.
        /// </summary>
        /// <returns></returns>
        PerformanceCounterCategory CreateCategory();

        /// <summary>
        /// Tries to Delete the Category.
        /// </summary>
        bool TryDeleteCategory();
    }

    internal static class CategoryExtensionMethods
    {
        internal static IPerformanceCounterCategoryDescriptor Register(this IPerformanceCounterCategoryDescriptor category
            , IPerformanceMeasurementDescriptor measurement)
        {
            category.Measurements.ToBidirectionalList(
                added => added.CategoryDescriptor = category
                , removed => removed.CategoryDescriptor = null).Add(measurement);
            return category;
        }

        internal static bool Unregister(this IPerformanceCounterCategoryDescriptor category,
            IPerformanceMeasurementDescriptor measurement)
        {
            return category.Measurements.ToBidirectionalList(
                onAfterRemoved: removed => { }).Remove(measurement);
        }
    }
}
