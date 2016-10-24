using System.Reflection;

namespace MeasureIt
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMeasurementOptions : IPublishingOptions, ISamplingOptions
    {
        /// <summary>
        /// 
        /// </summary>
        MethodInfo Method { get; set; }
    }
}
