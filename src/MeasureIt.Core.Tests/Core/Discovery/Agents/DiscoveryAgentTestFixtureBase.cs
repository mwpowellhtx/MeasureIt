using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public abstract class DiscoveryAgentTestFixtureBase<TAgent, T> : IntegrationTestFixtureBase
        where TAgent : DiscoveryAgentBase<T>
        where T : IDescriptor
    {
        /// <summary>
        /// Returns the <see cref="IEnumerable{Type}"/> corresponding to the
        /// <paramref name="getter"/> applied across the <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        protected static IEnumerable<Type> GetTypes(Func<Assembly, IEnumerable<Type>> getter,
            params Assembly[] assemblies)
        {
            return assemblies.SelectMany(getter);
        }

        protected virtual DiscoveryServiceExportedTypesGetterDelegate GetExportedTypes
        {
            get
            {
                return () =>
                {
                    var types = GetTypes(a => a.GetExportedTypes(),
                        typeof(Support.Root).Assembly,
                        typeof(IDescriptor).Assembly);

                    Assert.NotNull(types);
                    // ReSharper disable once PossibleMultipleEnumeration
                    Assert.NotEmpty(types);

                    // ReSharper disable once PossibleMultipleEnumeration
                    return types;
                };
            }
        }

        private IInstrumentationDiscoveryOptions _options;

        /// <summary>
        /// Gets the Options that derive the Test conditions.
        /// </summary>
        protected IInstrumentationDiscoveryOptions Options
        {
            get { return _options; }
            private set
            {
                Assert.NotNull(value);
                _options = value;
            }
        }

        private readonly Lazy<TAgent> _lazyAgent;

        /// <summary>
        /// Override to return the desired <typeparamref name="TAgent"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="getExportedTypes"></param>
        /// <returns></returns>
        protected abstract TAgent CreateAgent(IInstrumentationDiscoveryOptions options,
            DiscoveryServiceExportedTypesGetterDelegate getExportedTypes);

        protected DiscoveryAgentTestFixtureBase(IInstrumentationDiscoveryOptions options)
        {
            Options = options;
            _lazyAgent = new Lazy<TAgent>(() => CreateAgent(Options, GetExportedTypes));
        }

        /// <summary>
        /// Override to verify the <paramref name="discoveredItems"/>.
        /// </summary>
        /// <param name="discoveredItems"></param>
        protected abstract void OnItemsDiscovered(IEnumerable<T> discoveredItems);

        [Fact]
        public void DiscoveredItemsAreCorrect()
        {
            var agent = _lazyAgent.Value;
            Assert.NotNull(agent);
            OnItemsDiscovered(agent);
        }
    }
}
