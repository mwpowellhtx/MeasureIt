using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery
{
    using Contexts;

    /// <summary>
    /// Installer requires the Runtime Discovery Service as well as connecting the dots.
    /// </summary>
    public class InstallerInstrumentationDiscoveryService : RuntimeInstrumentationDiscoveryService
        , IInstallerInstrumentationDiscoveryService
    {
        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<IPerformanceCounterCategoryAdapter> _categoryAdapters;

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IPerformanceCounterCategoryAdapter> CategoryAdapters
        {
            get { return _categoryAdapters; }
            private set { _categoryAdapters = (value ?? new List<IPerformanceCounterCategoryAdapter>()).ToArray(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public InstallerInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
            CategoryAdapters = null;
        }

        public IInstallerContext GetInstallerContext()
        {
            return new InstallerContext(Options, this);
        }

        private void OnDiscoverCounterAdapterDescriptors()
        {
            CategoryAdapters = Measurements
                .GroupBy(d => d.CategoryType)
                .Select(g =>
                {
                    var category = g.Key.CreateInstance<IPerformanceCounterCategoryAdapter>();
                    foreach (var d in g)
                        category.InternalMeasurements.Add(d);
                    return category;
                });
        }

        protected override void OnDiscover()
        {
            base.OnDiscover();

            OnDiscoverCounterAdapterDescriptors();
        }
    }
}
