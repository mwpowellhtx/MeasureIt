using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt
{
    /// <summary>
    /// Category Adapter is the concrete type that facilitates the feature set for each
    /// <see cref="PerformanceCounterCategory"/>.
    /// </summary>
    public interface IPerformanceCounterCategoryAdapter
    {
        /// <summary>
        /// Gets the Name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Help.
        /// </summary>
        string Help { get; }

        /// <summary>
        /// Gets the <see cref="PerformanceCounterCategoryType"/>.
        /// </summary>
        PerformanceCounterCategoryType CategoryType { get; }

        /// <summary>
        /// Gets or sets the Measurements.
        /// </summary>
        IList<IPerformanceMeasurementDescriptor> Measurements { get; set; }

        /// <summary>
        /// Gets an <see cref="IList{IPerformanceMeasurementDescriptor}"/> for internal use.
        /// </summary>
        IList<IPerformanceMeasurementDescriptor> InternalMeasurements { get; }

        /// <summary>
        /// Gets a readonly collection of CreationData.
        /// </summary>
        IReadOnlyCollection<ICounterCreationDataDescriptor> CreationData { get; }
    }

    internal static class CategoryAdapterExtensionMethods
    {
        internal static TCategory Register<TCategory>(this TCategory category,
            IPerformanceMeasurementDescriptor measurement)
            where TCategory : class, IPerformanceCounterCategoryAdapter
        {
            category.InternalMeasurements.Add(measurement);
            return category;
        }

        internal static bool Unregister<TCategory>(this TCategory category,
            IPerformanceMeasurementDescriptor measurement)
            where TCategory : class, IPerformanceCounterCategoryAdapter
        {
            return category.InternalMeasurements.Remove(measurement);
        }
    }
}
