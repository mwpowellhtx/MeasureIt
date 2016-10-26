using System.Reflection;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// InstrumentationDiscoveryOptions class.
    /// </summary>
    public class InstrumentationDiscoveryOptions: IInstrumentationDiscoveryOptions
    {
        public int? RandomSeed { get; set; }

        public bool IncludeInherited { get; set; }

        public BindingFlags MethodBindingAttr { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InstrumentationDiscoveryOptions()
        {
            IncludeInherited = true;
            MethodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
        }
    }
}
