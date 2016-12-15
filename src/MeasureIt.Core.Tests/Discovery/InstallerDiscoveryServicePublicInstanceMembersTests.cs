using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class InstallerDiscoveryServicePublicInstanceMembersTests
        : InstallerDiscoveryServiceTestFixtureBase<
            InstallerInstrumentationDiscoveryService>
    {
        protected static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(Support.Root).Assembly;
            yield return typeof(IDescriptor).Assembly;
        }

        private readonly IInstrumentationDiscoveryOptions _options;

        protected override IInstrumentationDiscoveryOptions Options
        {
            get { return _options; }
        }

        protected override ServiceFactoryDelegate ServiceFactory
        {
            get { return o => new InstallerInstrumentationDiscoveryService(o); }
        }

        public InstallerDiscoveryServicePublicInstanceMembersTests()
        {
            _options = new InstrumentationDiscoveryOptions {Assemblies = GetAssemblies()}.VerifyOptions();
        }

        protected override void VerifyDiscoveredCounterAdapters(
            IEnumerable<IPerformanceCounterAdapter> discoveredItems)
        {
            var ordered = discoveredItems.Order().ToArray();

            ordered.Verify();
        }

        protected override void OnVerifyCategoryAdapters(
            IEnumerable<IPerformanceCounterCategoryAdapter> categories)
        {
            Assert.NotNull(categories);

            var orderedCategories = categories.Order().ToArray();

            const PerformanceCounterCategoryType multiInstance = PerformanceCounterCategoryType.MultiInstance;

            // Listed in order of enumerated integral value...
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;

            Assert.Collection(orderedCategories
                , c =>
                {
                    Assert.Equal(c.GetType().FullName, c.Name);
                    Assert.Null(c.Help);
                    Assert.Equal(multiInstance, c.CategoryType);

                    // TODO: TBD: how come this is losing the instances?
                    var orderedData = c.CreationData.Order().ToArray();

                    const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
                    const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
                    const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
                    const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
                    const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

                    const Accessibility @public = Accessibility.Public;

                    const Virtuality @virtual = Virtuality.Virtual;
                    const Virtuality @override = Virtuality.Override;

                    const string empty = "";

#if DEBUG
                    //var names = orderedData.Select(x => x.Name).OrderBy(x => x).ToArray();
#endif

                    // TODO: TBD: may need/want a more robust set of examples...
                    Assert.Collection(orderedData
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, @virtual, d.CounterType), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInBaseOnly.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, @virtual, averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, counterType: averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual, averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInDerivedClass.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual, averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInDerivedOnly.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, counterType: averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, counterType: averageTimer), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, @virtual, averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInBaseOnly.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, @virtual, averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                                SubjectClass>(VoidType, @public, counterType: averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual, averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInDerivedClass.BuildMethodSignature
                                <SubjectClassWithNonPublicMethods>(VoidType, @public, @virtual, averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(methodDeclaredInDerivedOnly.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, counterType: averageBase), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly.BuildMethodSignature<
                                SubjectClassWithNonPublicMethods>(VoidType, @public, counterType: averageBase), d.Name);
                        }
                        );
                }
                );
        }

        protected override void OnVerifyPerformanceMeasurements(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            const Accessibility @public = Accessibility.Public;

            const Virtuality @virtual = Virtuality.Virtual;

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
                                                                                                                                                    