using System.Reflection;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInstrumentationDiscoveryOptions
    {
        /// <summary>
        /// Gets or sets the RandomSeed.
        /// </summary>
        int? RandomSeed { get; set; }

        /// <summary>
        /// Gets or sets whether to IncludeInherited among the Reflected upon Classes, Types,
        /// Methods and Attributes.
        /// </summary>
        bool IncludeInherited { get; set; }

        /// <summary>
        /// Gets or sets the Method <see cref="BindingFlags"/>.
        /// </summary>
        BindingFlags MethodBindingAttr { get; set; }
    }
}
