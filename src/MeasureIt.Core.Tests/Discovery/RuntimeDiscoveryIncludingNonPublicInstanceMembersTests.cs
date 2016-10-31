using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryIncludingNonPublicInstanceMembersTests
        : RuntimeDiscoveryServiceTestFixtureBase<
            RuntimeInstrumentationDiscoveryService>
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            // These are NOT the default options.
            const BindingFlags expectedBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return new InstrumentationDiscoveryOptions {MethodBindingAttr = expectedBindingAttr}.VerifyOptions(expectedBindingAttr);
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
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            var voidType = typeof(void);

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";

            // The descriptors will have been presented in a predictable order.
            Assert.Collection(ordered
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(internalTargetMethod, d.Name);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(Constants.MinSampleRate);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, internalTargetMethod, false);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.Name);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }
}
