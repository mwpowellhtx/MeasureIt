using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryIncludingNonPublicInstanceMembersTests
        : RuntimeDiscoveryTestFixtureBase<RuntimeInstrumentationDiscoveryService>
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            // These are NOT the default options.
            const BindingFlags expectedBindingAttr =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions {MethodBindingAttr = expectedBindingAttr};
            Assert.NotNull(options);
            Assert.Equal(expectedBindingAttr, options.MethodBindingAttr);
            return options;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(Support.Root).Assembly;
            yield return typeof(IDescriptor).Assembly;
        }

        private static T[] VerifyCount<T>(IEnumerable<T> items, int expectedCount)
        {
            Assert.NotNull(items);
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Equal(expectedCount, items.Count());
            // ReSharper disable once PossibleMultipleEnumeration
            return items.ToArray();
        }

        public RuntimeDiscoveryIncludingNonPublicInstanceMembersTests()
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
            // Make sure that the Methods all have a verifiable ReflectedType.

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(descriptors.Select(d => d.Method.ReflectedType), Assert.NotNull);

            // ReSharper disable once PossibleMultipleEnumeration
            // ReSharper disable once PossibleNullReferenceException
            var ordered = descriptors.Order().ToArray();

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";

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
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(internalTargetMethod, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(Constants.MinSampleRate);
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, internalTargetMethod, false);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
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
