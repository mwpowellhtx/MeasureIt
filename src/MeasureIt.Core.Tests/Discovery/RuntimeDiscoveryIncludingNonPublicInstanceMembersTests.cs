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

        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            // These are NOT the default options.
            const BindingFlags expectedBindingAttr
                = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return new InstrumentationDiscoveryOptions
            {
                MethodBindingAttr = expectedBindingAttr,
                Assemblies = VerifyCount(GetAssemblies(), 2)
            }.VerifyOptions(expectedBindingAttr);
        }

        private readonly IInstrumentationDiscoveryOptions _options;

        protected override IInstrumentationDiscoveryOptions Options
        {
            get { return _options; }
        }

        protected override ServiceFactoryDelegate ServiceFactory
        {
            get { return o => new RuntimeInstrumentationDiscoveryService(o); }
        }

        public RuntimeDiscoveryIncludingNonPublicInstanceMembersTests()
        {
            _options = GetOptions();
        }

        protected override void VerifyDiscoveredCounterAdapters(
            IEnumerable<IPerformanceCounterAdapter> discoveredItems)
        {
            discoveredItems.Verify();
        }

        protected override void VerifyDiscoveredCounterDescriptors(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";

            const Accessibility @public = Accessibility.Public;
            const Accessibility @internal = Accessibility.Internal;

            const Virtuality @virtual = Virtuality.Virtual;

            // The descriptors will have been presented in a predictable order.
            Assert.Collection(ordered
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInBaseOnly);
                    Assert.Equal(virtualMethodDecoratedInBaseOnly.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClass>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(VoidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecoratedInDerivedClass);
                    Assert.Equal(virtualMethodDecoratedInDerivedClass.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(VoidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
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
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(VoidType, methodDeclaredInDerivedOnly);
                    Assert.Equal(methodDeclaredInDerivedOnly.BuildMethodSignature<
                        SubjectClassWithNonPublicMethods>(VoidType, @public), d.MemberSignature);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                );
        }
    }
}
