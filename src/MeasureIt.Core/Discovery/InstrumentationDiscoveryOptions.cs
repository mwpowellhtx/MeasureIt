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
        /// <summary>
        /// Gets or sets the RandomSeed.
        /// </summary>
        public int? RandomSeed { get; set; }

        /// <summary>
        /// Gets or sets whether to ThrowOnInstallerFailure.
        /// </summary>
        public bool ThrowOnInstallerFailure { get; set; }

        /// <summary>
        /// Gets or sets whether ot IncludeInherited.
        /// </summary>
        public bool IncludeInherited { get; set; }

        /// <summary>
        /// Gets or sets the ConstructorBindingAttr.
        /// </summary>
        public BindingFlags ConstructorBindingAttr { get; set; }

        /// <summary>
        /// Gets or sets the MethodBindingAttr.
        /// </summary>
        public BindingFlags MethodBindingAttr { get; set; }

        /// <summary>
        /// Gets or sets the Assemblies.
        /// </summary>
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
