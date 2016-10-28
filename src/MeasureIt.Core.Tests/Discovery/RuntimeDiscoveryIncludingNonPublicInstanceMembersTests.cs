using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryIncludingNonPublicInstanceMembersTests
        : RuntimeDiscoveryTestFixtureBase<RuntimeInstrumentationDiscoveryService>
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
            IEnumerable<IMeasurePerformanceDescriptor> descriptors)
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
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(
                        voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(internalTargetMethod, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(Constants.MinSampleRate);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, internalTargetMethod, false);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods, SubjectClassWithNonPublicMethods>(
                        voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                );
        }
    }
}
