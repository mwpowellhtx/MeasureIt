using System.Reflection;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// InstrumentationDiscovererOptions class.
    /// </summary>
    public class InstrumentationDiscovererOptions
    {
        /// <summary>
        /// Gets or sets whether to IncludeInherited among the Reflected upon Classes, Types,
        /// Methods and Attributes.
        /// </summary>
        public bool IncludeInherited { get; set; }

        /// <summary>
        /// Gets or sets the Method <see cref="BindingFlags"/>.
        /// </summary>
        public BindingFlags MethodBindingAttr { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InstrumentationDiscovererOptions()
        {
            IncludeInherited = true;
            MethodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
        }
    }
}
