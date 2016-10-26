using System;
using System.Collections.Generic;
using System.Diagnostics;
using MeasureIt.Measurement;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPerformanceCounterDescriptor
        : IMeasurementOptions
            , IDescriptor
            , IEquatable<IPerformanceCounterDescriptor>
    {
        /// <summary>
        /// Gets or sets the CategoryType.
        /// </summary>
        Type CategoryType { get; set; }

        /// <summary>
        /// Gets the CounterCategoryDescriptor.
        /// </summary>
        IPerformanceCounterCategoryDescriptor CategoryDescriptor { get; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        string CounterName { get; set; }

        /// <summary>
        /// Connect the <see cref="PerformanceCounter"/> with the adapter itself.
        /// </summary>
        Type AdapterType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        IPerformanceCounterAdapterDescriptor AdapterDescriptor { get; }

        /// <summary>
        /// 
        /// </summary>
        PerformanceCounterInstanceLifetime InstanceLifetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PerformanceCounter> GetPerformanceCounters();

        /// <summary>
        /// Returns a new <see cref="IPerformanceCounterAdapter"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        IPerformanceCounterAdapter CreateAdapter();

        /// <summary>
        /// Returns a new <see cref="IPerformanceCounterContext"/> corresponding to the Descriptor.
        /// </summary>
        /// <returns></returns>
        IPerformanceCounterContext CreateContext();
    }
}
