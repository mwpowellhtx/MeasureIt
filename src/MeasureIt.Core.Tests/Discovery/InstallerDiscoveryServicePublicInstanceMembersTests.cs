using System;
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

                    const string dot = ".";
                    const string empty = "";
                    const string @base = "Base";

#if DEBUG
                    //var names = orderedData.Select(x => x.Name).OrderBy(x => x).ToArray();
#endif

                    // TODO: TBD: may need/want a more robust set of examples...
                    Assert.Collection(orderedData
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed.PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInBaseOnly.PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly.PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecorationOvershadowed
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(virtualMethodDecoratedInDerivedClass
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInDerivedOnly
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageTimer, d.CounterType);
                            Assert.Equal(methodDeclaredInBaseOnly
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, virtualMethodDecorationOvershadowed, @base)
                                .PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, virtualMethodDecoratedInBaseOnly, @base)
                                .PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, methodDeclaredInBaseOnly, @base)
                                .PrefixName<SubjectClass>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, virtualMethodDecorationOvershadowed, @base)
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, virtualMethodDecoratedInDerivedClass, @base)
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, methodDeclaredInDerivedOnly, @base)
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        , d =>
                        {
                            Assert.Equal(empty, d.Help);
                            Assert.Equal(averageBase, d.CounterType);
                            Assert.Equal(string.Join(dot, methodDeclaredInBaseOnly, @base)
                                .PrefixName<SubjectClassWithNonPublicMethods>(), d.Name);
                        }
                        );
                }
                );
        }

        protected override void OnVerifyPerformanceMeasurements(
            IEnumerable<IPerformanceMeasurementDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            var voidType = typeof(void);

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            //const string empty = "";
            //const string dot = ".";
            //const string @base = "Base";

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
                                                                                                                                                    