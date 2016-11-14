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

                    // TODO: TBD: may need/want a more robust set of examples...
                    Assert.Collection(orderedData
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
                        }
                        , x =>
                        {
                            x.Name.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.NotNull(x.Help);
                            Assert.Empty(x.Help);
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

            Assert.Collection(ordered
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.Name);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.Name);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterCategoryAdapter<DefaultPerformanceCounterCategoryAdapter>();
                }
                );
        }
    }
}
                                                                                                                                                    