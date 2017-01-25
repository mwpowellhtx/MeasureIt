//// TODO: TBD: instead of options, decided to put this in the attributes
//namespace MeasureIt.Discovery
//{
//    /// <summary>
//    /// Establishes <see cref="InstrumentationDiscoveryOptions"/> for use with ASP.NET MVC
//    /// performance measurement.
//    /// </summary>
//    public partial class MvcInstrumentationDiscoveryOptions : InstrumentationDiscoveryOptions
//        , IMvcInstrumentationDiscoveryOptions
//    {
//        /// <summary>
//        /// Gets or sets the <see cref="MeasurementBoundaryPair"/> Boundary.
//        /// </summary>
//        public MeasurementBoundaryPair Boundary { get; set; }

//        /// <summary>
//        /// Sets the <see cref="Boundary"/> given <paramref name="start"/> and <paramref name="stop"/>.
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="stop"></param>
//        public void SetBoundary(MeasurementBoundary start, MeasurementBoundary stop)
//        {
//            Boundary = new MeasurementBoundaryPair(start, stop);
//        }

//        /// <summary>
//        /// Sets the <see cref="Boundary"/> given <paramref name="value"/>.
//        /// </summary>
//        /// <param name="value"></param>
//        public void SetBoundary(MeasurementBoundaryPair value)
//        {
//            Boundary = value;
//        }
//    }
//}
