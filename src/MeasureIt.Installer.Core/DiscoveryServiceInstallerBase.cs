using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Discovery;

    /// <summary>
    /// Provides <see cref="IInstrumentationDiscoveryService"/>
    /// <see cref="System.Configuration.Install.Installer"/> hooks. All you need to do is
    /// reference the appropriate set of <see cref="Assembly"/>(ies) and the discovery service.
    /// </summary>
    [Description("MeasureIt Discovery Service Installer")]
    public abstract class DiscoveryServiceInstallerBase : Installer, IDiscoveryServiceInstaller
    {
        private readonly IInstallerInstrumentationDiscoveryService _discoveryService;

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="discoveryService"></param>
        protected DiscoveryServiceInstallerBase(IInstallerInstrumentationDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            Func<string, bool> exists = PerformanceCounterCategory.Exists;

            foreach (var c in _discoveryService.CategoryDescriptors.Where(d => !exists(d.Name)))
            {
                var data = c.GetCounterCreationDataCollection();
                PerformanceCounterCategory.Create(c.Name, c.Help, c.CategoryType, data);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            Func<string, bool> exists = PerformanceCounterCategory.Exists;
            Action<string> delete = PerformanceCounterCategory.Delete;

            foreach (var category in _discoveryService.CategoryDescriptors.Where(d => exists(d.Name)))
                delete(category.Name);

            base.Uninstall(savedState);
        }
    }
}
