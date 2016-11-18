using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class RuntimeDiscoveryServicePublicInstanceMembersTests
        : RuntimeDiscoveryServiceTestFixtureBase<
            RuntimeInstrumentationDiscoveryService>
    {
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

        private static IInstrumentationDiscoveryOptions GetOptions()
        {
            return new InstrumentationDiscoveryOptions {Assemblies = VerifyCount(GetAssemblies(), 2)}.VerifyOptions();
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

        public RuntimeDiscoveryServicePublicInstanceMembersTests()
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
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.PrefixName<SubjectClass>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    Assert.Equal(virtualMethodDecoratedInBaseOnly.PrefixName<SubjectClass>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed.PrefixName<SubjectClass>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    Assert.Equal(methodDeclaredInBaseOnly.PrefixName<SubjectClassWithNonPublicMethods>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInDerivedClass);
                    Assert.Equal(virtualMethodDecoratedInDerivedClass.PrefixName<SubjectClassWithNonPublicMethods>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    Assert.Equal(virtualMethodDecorationOvershadowed
                        .PrefixName<SubjectClassWithNonPublicMethods>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
                    Assert.Equal(methodDeclaredInDerivedOnly
                        .PrefixName<SubjectClassWithNonPublicMethods>(), d.Prefix);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                );
        }
    }
}
