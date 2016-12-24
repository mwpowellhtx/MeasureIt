using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MeasureIt.Discovery.Agents
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DiscoveryAgentBase<T> : IDiscoveryAgent<T>
        where T : class
    {
        static DiscoveryAgentBase()
        {
            var type = typeof(T);

            if (type.IsInterface) return;

            var message = string.Format("{0} type must be an interface.", type);
            throw new InvalidOperationException(message) {Data = {{"type", type}}};
        }

        private readonly IInstrumentationDiscoveryOptions _options;

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
        protected DiscoveryAgentBase(IInstrumentationDiscoveryOptions options,
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
        protected virtual IEnumerable<T> DiscoverValues(IInstrumentationDiscoveryOptions options,
            IEnumerable<Type> exportedTypes)
        {
            /* If the end user is evaluating a discovery agent it is because discovery is
             * desired, which requires exported types. */

            // ReSharper disable once InvertIf
            if (!exportedTypes.Any())
            {
                const string message = "Proper discovery requires one or more exported types.";
                throw new ArgumentException(message, nameof(exportedTypes));
            }

            yield break;
        }

        /// <summary>
        /// Returns the enumerator for the Discovery agent.
        /// </summary>
        /// <returns></returns>
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
