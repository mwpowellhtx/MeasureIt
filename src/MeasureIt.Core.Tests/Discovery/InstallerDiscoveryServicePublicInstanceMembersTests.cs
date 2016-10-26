using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery
{
    using Xunit;

    public class InstallerDiscoveryServicePublicInstanceMembersTests
        : InstallerDiscoveryTestFixtureBase<
            InstallerInstrumentationDiscoveryService>
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingFlags = BindingFlags.Public | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions();
            Assert.NotNull(options);
            Assert.Equal(methodBindingFlags, options.MethodBindingAttr);
            return options;
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(Support.Root).Assembly;
            yield return typeof(IDescriptor).Assembly;
        }

        private static InstallerInstrumentationDiscoveryService CreateService(
            InstrumentationDiscovererOptions options, IEnumerable<Assembly> assemblies)
        {
            return new InstallerInstrumentationDiscoveryService(options, assemblies);
        }

        public InstallerDiscoveryServicePublicInstanceMembersTests()
            : base(GetOptions(), GetAssemblies(), CreateService)
        {
        }

        protected override void VerifyDiscoveredCounterAdapterDescriptors(
            IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            ordered.Verify();
        }

        protected override void OnVerifyCategoryDescriptors(
            IEnumerable<IPerformanceCounterCategoryDescriptor> descriptors)
        {
            Assert.NotNull(descriptors);

            var orderedCategories = descriptors.OrderBy(x => x.Name).ThenBy(x => x.CategoryType).ToArray();

            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;
            const PerformanceCounterCategoryType multiInstance = PerformanceCounterCategoryType.MultiInstance;

            // Listed in order of enumerated integral value...
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;

            Assert.Collection(orderedCategories,
                d =>
                {
                    d.Name.CanParse<string, Guid>(Guid.TryParse);
                    d.Type.Confirm<DefaultPerformanceCounterCategoryAdapter>();
                    Assert.Equal(string.Empty, d.Help);
                    Assert.Equal(multiInstance, d.CategoryType);

                    // TODO: TBD: how come this is losing the instances?
                    var orderedData = d.CreationDataDescriptors
                        .OrderBy(x => x.CounterType).ThenBy(x => x.Help).ToArray();

                    // TODO: TBD: may need/want a more robust set of examples...
                    Assert.Collection(orderedData,
                        x =>
                        {
                            x.CounterName.CanParse<string, Guid>(Guid.TryParse);
                            x.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(process, x.InstanceLifetime);
                            Assert.Equal(averageTimer, x.CounterType);
                            Assert.Null(x.Help);
                            Assert.Null(x.ReadOnly);
                        }
                        , x =>
                        {
                            x.CounterName.CanParse<string, Guid>(Guid.TryParse);
                            x.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(process, x.InstanceLifetime);
                            Assert.Equal(averageBase, x.CounterType);
                            Assert.Null(x.Help);
                            Assert.Null(x.ReadOnly);
                        }
                        );
                }
                );
        }

        protected override void OnVerifyCounterDescriptors(IEnumerable<IPerformanceCounterDescriptor> descriptors)
        {
            var ordered = descriptors.Order().ToArray();

            var voidType = typeof(void);

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            Assert.Collection(ordered,
                d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions(expectedReadOnly: true);
                    d.RootType.Confirm<SubjectClass>();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClass
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInDerivedClass);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(0.25d, false);
                    d.RootType.Confirm<SubjectClassWithNonPublicMethods>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                );
        }
    }
}
                                                                                                                                                    