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
            typeof(T).VerifyIsInterface();
        }

        private readonly IInstrumentationDiscoveryOptions _discoveryOptions;

        private readonly DiscoveryServiceExportedTypesGetterDelegate _getExportedTypes;

        private static IEnumerable<Type> DefaultGetExportedTypes()
        {
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="discoveryOptions"></param>
        /// <param name="getExportedTypes"></param>
        protected DiscoveryAgentBase(IInstrumentationDiscoveryOptions discoveryOptions,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes)
        {
            _discoveryOptions = discoveryOptions;
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
            return DiscoverValues(_discoveryOptions, _getExportedTypes()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
