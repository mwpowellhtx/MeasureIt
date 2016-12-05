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
        /// Gets or sets the Prefix.
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// Connect the <see cref="PerformanceCounter"/> with the adapters themselves.
        /// </summary>
        IEnumerable<Type> AdapterTypes { get; }

        /// <summary>
        /// Gets the <see cref="IPerformanceCounterAdapter"/> corresponding with the <see cref="AdapterTypes"/>.
        /// </summary>
        IEnumerable<IPerformanceCounterAdapter> Adapters { get; }

        /// <summary>
        /// 
        /// </summary>
        PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// Gets the Exception which occurred during invocation of the target method.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets whether the Descriptor Has <see cref="Exception"/>.
        /// </summary>
        bool HasError { get; }

        /// <summary>
        /// Sets the <see cref="Exception"/> which occurred during invocation of the target method.
        /// </summary>
        /// <param name="ex"></param>
        void SetError(Exception ex = null);

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

        ///// <summary>
        ///// Gets the <see cref="CounterCreationData"/> associated with this Descriptor, provided
        ///// to <see cref="IPerformanceCounterCategoryDescriptor"/> for creation purposes.
        ///// </summary>
        //IEnumerable<CounterCreationData> Data { get; }
    }
}
