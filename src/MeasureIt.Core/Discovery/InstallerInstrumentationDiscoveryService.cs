using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Agents;

    /// <summary>
    /// Installer requires the Runtime Discovery Service as well as connecting the dots.
    /// </summary>
    public class InstallerInstrumentationDiscoveryService : RuntimeInstrumentationDiscoveryService
        , IInstallerInstrumentationDiscoveryService
    {
        private readonly Lazy<IPerformanceCounterCategoryDescriptorDiscoveryAgent>
            _performanceCounterCategoryDescriptorDiscoveryAgent;

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<IPerformanceCounterCategoryDescriptor> _categoryDescriptors;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPerformanceCounterCategoryDescriptor> CategoryDescriptors
        {
            get {return _categoryDescriptors;}
            private set
            {
                _categoryDescriptors = (value ?? new List<IPerformanceCounterCategoryDescriptor>()).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : this(options, new List<Assembly>())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="assemblies"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options,
            IEnumerable<Assembly> assemblies)
            : base(options, assemblies)
        {
            CategoryDescriptors = null;

            _performanceCounterCategoryDescriptorDiscoveryAgent
                = new Lazy<IPerformanceCounterCategoryDescriptorDiscoveryAgent>(
                    () => new PerformanceCounterCategoryDescriptorDiscoveryAgent(options, GetExportedTypes));
        }

        private void OnDiscoverCounterAdapterDescriptors()
        {
            CategoryDescriptors = _performanceCounterCategoryDescriptorDiscoveryAgent.Value.ToArray();
        }

        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverCounterAdapterDescriptors();

            // TODO: now we must align the categories with the adapters...

            var counters = CounterDescriptors.ToArray();
            var adapters = CounterAdapterDescriptors.ToArray();

            foreach (var category in CategoryDescriptors)
            {
                var c = category;

                var adaptersInUse = adapters.Where(x => counters.Any(
                    y => y.AdapterTypes.Contains(x.AdapterType) && y.CategoryType == c.Type));

                if (!adapters.Any()) continue;

                // Make sure that the Descriptors are presented in the correct order.
                c.CreationDataDescriptors = adaptersInUse.SelectMany(a => a.CreationDataDescriptors);
            }
        }
    }
}
