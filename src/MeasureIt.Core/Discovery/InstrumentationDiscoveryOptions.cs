using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    /// <summary>
    /// InstrumentationDiscoveryOptions class.
    /// </summary>
    public class InstrumentationDiscoveryOptions: IInstrumentationDiscoveryOptions
    {
        public int? RandomSeed { get; set; }

        public bool ThrowOnInstallerFailure { get; set; }

        public bool IncludeInherited { get; set; }

        public BindingFlags ConstructorBindingAttr { get; set; }

        public BindingFlags MethodBindingAttr { get; set; }

        public IEnumerable<Assembly> Assemblies { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public InstrumentationDiscoveryOptions()
            : this(new List<Assembly>())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InstrumentationDiscoveryOptions(IEnumerable<Assembly> assemblies)
        {
            ThrowOnInstallerFailure = true;
            IncludeInherited = true;
            MethodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
            ConstructorBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Assemblies = (assemblies ?? new Assembly[0]).ToArray();
        }
    }
}
