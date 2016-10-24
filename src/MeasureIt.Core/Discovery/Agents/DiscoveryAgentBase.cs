using System;
using System.Collections;
using System.Collections.Generic;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DiscoveryAgentBase<T> : IDiscoveryAgent<T>
        where T : IDescriptor
    {
        private readonly InstrumentationDiscovererOptions _options;

        private readonly DiscoveryServiceExportedTypesGetterDelegate _getExportedTypes;

        private static IEnumerable<Type> DefaultGetExportedTypes()
        {
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="getExportedTypes"></param>
        protected DiscoveryAgentBase(InstrumentationDiscovererOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            _options = options;
            _getExportedTypes = getExportedTypes ?? DefaultGetExportedTypes;
        }

        /// <summary>
        /// Override to Discover the Values.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="exportedTypes"></param>
        /// <returns></returns>
        protected abstract IEnumerable<T> DiscoverValues(InstrumentationDiscovererOptions options,
            IEnumerable<Type> exportedTypes);

        public IEnumerator<T> GetEnumerator()
        {
            return DiscoverValues(_options, _getExportedTypes()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
