using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryServicePublicInstanceMembersTests : RuntimeDiscoveryTestFixtureBase<
        RuntimeInstrumentationDiscoveryService>
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions().VerifyOptions();
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
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var ordered = descriptors.Order().ToArray();

            var voidType = typeof(void);

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

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
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
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
                    Assert.Equal(methodDeclaredInDerivedOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
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
                    d.VerifyPublishingOptions(false, false, true).VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }
}
