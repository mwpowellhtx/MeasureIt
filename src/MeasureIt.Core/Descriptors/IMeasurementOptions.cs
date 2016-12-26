using System;
using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementOptions : IPublishingOptions, ISamplingOptions
    {
        /// <summary>
        /// Gets or sets the RootType. Due to the strategy in terms of how
        /// </summary>
        Type RootType { get; set; }

        // TODO: TBD: may need/want to reconsider how this is actually discovered, beyond the recursive method (?)
        /// <summary>
        /// 
        /// </summary>
        MethodInfo Method { get; set; }
    }
}
