using System;

namespace MeasureIt
{
    using Discovery;

    /// <summary>
    /// <see cref="PerformanceMeasurementDescriptor"/> for use with the Mvc framework.
    /// </summary>
    public class MvcPerformanceMeasurementDescriptor : PerformanceMeasurementDescriptor
        , IMvcPerformanceMeasurementDescriptor
    {
        /// <summary>
        /// Gets the Boundary <see cref="MeasurementBoundaryPair"/>.
        /// </summary>
        public virtual MeasurementBoundaryPair Boundary { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public MvcPerformanceMeasurementDescriptor(Type categoryType, Type adapterType, params Type[] otherAdapterTypes)
            : this(null, categoryType, adapterType, otherAdapterTypes)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="categoryType"></param>
        /// <param name="adapterType"></param>
        /// <param name="otherAdapterTypes"></param>
        public MvcPerformanceMeasurementDescriptor(string prefix, Type categoryType, Type adapterType,
            params Type[] otherAdapterTypes)
            : base(prefix, categoryType, adapterType, otherAdapterTypes)
        {
            Boundary = new MeasurementBoundaryPair();
        }
    }
}
