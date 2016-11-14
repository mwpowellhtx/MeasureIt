using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Contexts;
    using DataAttribute = CounterCreationDataAttribute;

    /// <summary>
    /// 
    /// </summary>
    public class RuntimeInstrumentationDiscoveryService : InstrumentationDiscoveryServiceBase
        , IRuntimeInstrumentationDiscoveryService
    {
        private readonly IDictionary<Type, IPerformanceCounterCategoryAdapter> _categoryAdapters
            = new ConcurrentDictionary<Type, IPerformanceCounterCategoryAdapter>();

        private IDictionary<Type, IPerformanceCounterCategoryAdapter> CategoryAdapters
        {
            get { return _categoryAdapters; }
        }


        private void RegisterCategoryAdapters(
            IDictionary<Type, IPerformanceCounterCategoryAdapter> adapters
            , IEnumerable<IPerformanceMeasurementDescriptor> measurements)
        {
            adapters.Clear();

            var bindingAttr = Options.ConstructorBindingAttr;

            // We want to consolidate the Counter Descriptors under single instances of each Category Adapter Type.
            foreach (var m in measurements)
            {
                var t = m.CategoryType;

                var adapter = adapters.ContainsKey(t)
                    ? adapters[t]
                    : (adapters[t] = t.CreateInstance<IPerformanceCounterCategoryAdapter>(bindingAttr));

                adapter.InternalMeasurements.Add(m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RuntimeInstrumentationDiscoveryService(IInstrumentationDiscoveryOptions options)
            : base(options)
        {
        }

        protected override void OnDiscover()
        {
            base.OnDiscover();

            RegisterCategoryAdapters(CategoryAdapters, Measurements);
        }

        /* TODO: TBD: we may need/want different discovery services for different purposes:
         * i.e. does a runtime discovery service really require the category descriptors?
         * that's just for install/uninstall purposes methinks... */

        public IMeasurementContext GetMeasurementContext()
        {
            return null;
        }

        public IMeasurementContext GetMeasurementContext(Type targetType, MethodInfo methodInfo)
        {
            return new MeasurementContext(null, null);
        }
    }
}
