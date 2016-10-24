using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryServicePublicInstanceMembersTests : RuntimeDiscoveryTestFixtureBase<
        RuntimeInstrumentationDiscoveryService>
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            // These are the DEFAULT options.
            const BindingFlags expectedBindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions();
            Assert.NotNull(options);
            Assert.Equal(expectedBindingAttr, options.MethodBindingAttr);
            return options;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(Support.Root).Assembly;
            yield return typeof(IDescriptor).Assembly;
        }

        // ReSharper disable once UnusedParameter.Local
        private static T[] VerifyCount<T>(IEnumerable<T> items, int expectedCount)
        {
            Assert.NotNull(items);
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Equal(expectedCount, items.Count());
            // ReSharper disable once PossibleMultipleEnumeration
            return items.ToArray();
        }

        public RuntimeDiscoveryServicePublicInstanceMembersTests()
            : base(GetOptions(), VerifyCount(GetAssemblies(), 2),
                (o, a) => new RuntimeInstrumentationDiscoveryService(o, a))
        {
        }

        protected override void VerifyDiscoveredCounterAdapterDescriptors(
            IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors)
        {
            descriptors.Verify();
        }

        protected override void VerifyDiscoveredCounterDescriptors(
            IEnumerable<IPerformanceCounterDescriptor> descriptors)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var ordered = descriptors.Order().ToArray();

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(descriptors.Select(d => d.Method.ReflectedType), Assert.NotNull);

            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            var voidType = typeof(void);

            // The descriptors will have been presented in a predictable order.
            Assert.Collection(ordered,
                d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    /* TODO: I keep looking at this, and thinking: this is the use case I am trying to avoid, is it not? where I want to "overshadow" attributes of a certain persuasion, on a given method, where Category and Adapter have already appeared.
                     * the difficulty is we are unable to discriminate baesd on reflecting/declaring type, apparently; can we depend upon attributes appearing in the correct? */
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }
}
