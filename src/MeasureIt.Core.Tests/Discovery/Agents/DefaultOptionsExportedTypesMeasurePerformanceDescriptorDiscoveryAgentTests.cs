using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public class DefaultOptionsExportedTypesMeasurePerformanceDescriptorDiscoveryAgentTests
        : MeasurePerformanceDescriptorDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions().VerifyOptions();
        }

        public DefaultOptionsExportedTypesMeasurePerformanceDescriptorDiscoveryAgentTests()
            : base(GetOptions())
        {
        }

        protected override void VerifyMethods(IEnumerable<MethodInfo> methods)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            base.VerifyMethods(methods);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.NotEmpty(methods);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(methods, m =>
            {
                Assert.NotNull(m);
                Assert.NotNull(m.ReflectedType);
                Assert.NotNull(m.ReflectedType.FullName);
            });
        }

        protected override void OnItemsDiscovered(IEnumerable<IMeasurePerformanceDescriptor> discoveredItems)
        {
            var orderedItems = discoveredItems.Order().ToArray();

            var voidType = typeof(void);

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            Assert.Collection(orderedItems
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
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
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
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
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
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
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
                    d.VerifyPublishingOptions(false, false, true).VerifySamplingOptions(0.25d, false);
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

    public class IncludingNonPublicOptionsExportedTypesMeasurePerformanceDescriptorDiscoveryAgentTests
        : MeasurePerformanceDescriptorDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return new InstrumentationDiscoveryOptions {MethodBindingAttr = methodBindingAttr}.VerifyOptions(methodBindingAttr);
        }

        public IncludingNonPublicOptionsExportedTypesMeasurePerformanceDescriptorDiscoveryAgentTests()
            : base(GetOptions())
        {
        }

        protected override void VerifyMethods(IEnumerable<MethodInfo> methods)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            base.VerifyMethods(methods);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.NotEmpty(methods);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(methods, m =>
            {
                Assert.NotNull(m);
                Assert.NotNull(m.ReflectedType);
                Assert.NotNull(m.ReflectedType.FullName);
            });
        }

        protected override void OnItemsDiscovered(IEnumerable<IMeasurePerformanceDescriptor> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            var orderedItems = discoveredItems.Order().ToArray();

            var voidType = typeof(void);

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            Assert.Collection(orderedItems
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
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
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
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
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
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, internalTargetMethod, false);
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
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
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
                    d.Method.Verify<SubjectClassWithNonPublicMethods,
                        SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true).VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter();
                    Assert.Collection(d.AdapterDescriptors
                        , a => Assert.Null(a.AdapterType)
                        );
                });
        }
    }
}
