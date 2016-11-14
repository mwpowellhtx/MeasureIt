using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MeasureIt
{
    using Contexts;

    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceMeasurementDescriptor
        : IMeasurementOptions
            , IDescriptor
            , IEquatable<IPerformanceMeasurementDescriptor>
    {
        /// <summary>
        /// Gets or sets the CategoryType.
        /// </summary>
        Type CategoryType { get; set; }

        /// <summary>
        /// Gets or sets the CategoryAdapter.
        /// </summary>
        IPerformanceCounterCategoryAdapter CategoryAdapter { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Connect the <see cref="PerformanceCounter"/> with the adapters themselves.
        /// </summary>
        IEnumerable<Type> AdapterTypes { get; set; }

        /// <summary>
        /// Gets the <see cref="IPerformanceCounterAdapter"/> corresponding with the <see cref="AdapterTypes"/>.
        /// </summary>
        IEnumerable<IPerformanceCounterAdapter> Adapters { get; }

        /// <summary>
        /// Gets or sets the AdapterNames.
        /// </summary>
        IEnumerable<string> AdapterNames { get; set; }

            /// <summary>
        /// 
        /// </summary>
        PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        //// TODO: TBD: no need to expose this via interface...
        ///// <summary>
        ///// Returns a new set of <see cref="IPerformanceCounterAdapter"/> instances corresponding to the Descriptor.
        ///// </summary>
        ///// <returns></returns>
        //IEnumerable<IPerformanceCounterAdapter> CreateAdapters();

        /// <summary>
        /// Returns a new <see cref="IPerformanceMeasurementContext"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        IPerformanceMeasurementContext CreateContext();

        /// <summary>
        /// Gets the <see cref="CounterCreationData"/> associated with this Descriptor, provided
        /// to <see cref="IPerformanceCounterCategoryDescriptor"/> for creation purposes.
        /// </summary>
        IEnumerable<CounterCreationData> Data { get; }
    }
}
