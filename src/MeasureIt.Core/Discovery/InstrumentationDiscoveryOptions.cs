using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using static BindingFlags;

    /// <summary>
    /// InstrumentationDiscoveryOptions class.
    /// </summary>
    public class InstrumentationDiscoveryOptions : IInstrumentationDiscoveryOptions
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
        /// Gets or sets whether to ThrowOnUninstallerFailure.
        /// </summary>
        public bool ThrowOnUninstallerFailure { get; set; }

        /// <summary>
        /// Gets or sets whether ot IncludeInherited.
        /// </summary>
        public bool IncludeInherited { get; set; }

        ///// <summary>
        ///// Gets or sets whether to PrepareCategoryName.
        ///// </summary>
        //public bool PrepareCategoryName { get; set; }

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
            ThrowOnUninstallerFailure = true;
            IncludeInherited = true;
            // PrepareCategoryName = true;
            MethodBindingAttr = Public | Instance;
            ConstructorBindingAttr = Public | NonPublic | Instance;
            Assemblies = (assemblies ?? new Assembly[0]).ToArray();
        }

        internal static TOptions CreateDefaultDiscoveryOptions<TOptions>()
            where TOptions : class, IInstrumentationDiscoveryOptions, new()
        {
            return new TOptions();
        }
    }
}
