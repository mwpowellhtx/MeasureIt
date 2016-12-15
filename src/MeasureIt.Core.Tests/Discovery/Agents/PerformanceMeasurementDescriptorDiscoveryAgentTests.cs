using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public class DefaultOptionsExportedTypesPerformanceMeasurementDescriptorDiscoveryAgentTests
        : PerformanceMeasurementDescriptorDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions()
                .VerifyOptions(
                    verify: o => Assert.Empty(o.Assemblies)
                );
        }

        public DefaultOptionsExportedTypesPerformanceMeasurementDescriptorDiscoveryAgentTests()
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

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceMeasurementDescriptor> discoveredItems)
        {
            var orderedItems = discoveredItems.Order().ToArray();

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            const Accessibility @public = Accessibility.Public;

            const Virtuality @virtual = Virtuality.Virtual;

            Assert.Collection(orderedItems
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInBaseOnly);
                    Assert.Equal(virtualMethodDecoratedInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInDerivedClass);
                    Assert.Equal(virtualMethodDecoratedInDerivedClass.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(VoidType, methodDeclaredInDerivedOnly);
                    Assert.Equal(methodDeclaredInDerivedOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }

    public class IncludingNonPublicOptionsExportedTypesPerformanceMeasurementDescriptorDiscoveryAgentTests
        : PerformanceMeasurementDescriptorDiscoveryAgentTestFixtureBase
    {
        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return new InstrumentationDiscoveryOptions {MethodBindingAttr = methodBindingAttr}.VerifyOptions(methodBindingAttr);
        }

        public IncludingNonPublicOptionsExportedTypesPerformanceMeasurementDescriptorDiscoveryAgentTests()
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

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceMeasurementDescriptor> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            var orderedItems = discoveredItems.Order().ToArray();

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            const Accessibility @public = Accessibility.Public;
            const Accessibility @internal = Accessibility.Internal;

            const Virtuality @virtual = Virtuality.Virtual;

            Assert.Collection(orderedItems
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInBaseOnly);
                    Assert.Equal(virtualMethodDecoratedInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInDerivedClass);
                    Assert.Equal(virtualMethodDecoratedInDerivedClass.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    // Remember, while we might expect the method to be Overridden, we are in fact dealing a the Virtual Base.
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(Constants.MinSampleRate);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(VoidType, internalTargetMethod, false);
                    Assert.Equal(internalTargetMethod.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @internal), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(VoidType, methodDeclaredInDerivedOnly);
                    Assert.Equal(methodDeclaredInDerivedOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }
}
