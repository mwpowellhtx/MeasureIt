using System;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery
{
    using Agents;
    using Contexts;

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
                _categoryDescriptors = (value ?? new IPerformanceCounterCategoryDescriptor[0]).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
            CategoryDescriptors = null;

            _performanceCounterCategoryDescriptorDiscoveryAgent
                = new Lazy<IPerformanceCounterCategoryDescriptorDiscoveryAgent>(
                    () => new PerformanceCounterCategoryDescriptorDiscoveryAgent(options, GetExportedTypes));
        }

        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(Options, this);
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

            var measurements = MeasurementDescriptors.ToArray();
            var adapters = CounterAdapterDescriptors.ToArray();

            foreach (var category in CategoryDescriptors)
            {
                var c = category;

                var adaptersInUse = adapters.Where(x => measurements.Any(
                    y => y.AdapterTypes.Contains(x.AdapterType) && y.CategoryType == c.Type));

                if (!adapters.Any()) continue;

                // Make sure that the Descriptors are presented in the correct order.
                c.CreationDataDescriptors = adaptersInUse.SelectMany(a => a.CreationDataDescriptors);
            }
        }
    }
}
