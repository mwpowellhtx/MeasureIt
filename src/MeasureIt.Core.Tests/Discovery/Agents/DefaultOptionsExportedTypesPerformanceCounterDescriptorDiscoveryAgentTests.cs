using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt.Discovery.Agents
{
    using Discovery;
    using Xunit;

    public class DefaultOptionsExportedTypesPerformanceCounterDescriptorDiscoveryAgentTests
        : PerformanceCounterDescriptorDiscoveryAgentTestFixtureBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions();
            Assert.NotNull(options);
            Assert.Equal(methodBindingAttr, options.MethodBindingAttr);
            return options;
        }

        public DefaultOptionsExportedTypesPerformanceCounterDescriptorDiscoveryAgentTests()
            : base(GetOptions())
        {
        }

        /// <summary>
        /// Apply ordering to the <paramref name="discoveredItems"/> while also performing
        /// a couple of preliminary verifications.
        /// </summary>
        /// <param name="discoveredItems"></param>
        /// <returns></returns>
        private static IEnumerable<IPerformanceCounterDescriptor> ApplyOrdering(
            IEnumerable<IPerformanceCounterDescriptor> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.NotEmpty(discoveredItems);

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(discoveredItems, x =>
            {
                Assert.NotNull(x.Method);
                Assert.NotNull(x.Method.Name);
                Assert.NotNull(x.Method.ReflectedType);
                Assert.NotNull(x.Method.ReflectedType.FullName);
                Assert.NotNull(x.Method.DeclaringType);
                Assert.NotNull(x.Method.DeclaringType.FullName);
            });

            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            return discoveredItems
                .OrderBy(x => x.CategoryType.FullName)
                .ThenBy(x => x.AdapterType.FullName)
                .ThenBy(x => x.Method.ReflectedType.FullName)
                .ThenBy(x => x.Method.Name);
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

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceCounterDescriptor> discoveredItems)
        {
            var orderedItems = ApplyOrdering(discoveredItems).ToArray();

            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            var voidType = typeof(void);

            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            Assert.Collection(orderedItems,
                d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {

                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true);
                    Assert.Equal(0.25d, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.False(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.CategoryType.Confirm<DefaultPerformanceCounterCategoryAdapter>();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions();
                    Assert.Equal(Constants.MaxSampleRate, d.SampleRate);
                    Assert.Equal(process, d.InstanceLifetime);
                    Assert.Null(d.ReadOnly);
                    Assert.Null(d.RandomSeed);
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                }
                );
        }
    }

    public class IncludingNonPublicOptionsExportedTypesPerformanceCounterDescriptorDiscoveryAgentTests
        : PerformanceCounterDescriptorDiscoveryAgentTestFixtureBase
    {
        private static InstrumentationDiscovererOptions GetOptions()
        {
            const BindingFlags methodBindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var options = new InstrumentationDiscovererOptions {MethodBindingAttr = methodBindingAttr};
            Assert.NotNull(options);
            Assert.Equal(methodBindingAttr, options.MethodBindingAttr);
            return options;
        }

        public IncludingNonPublicOptionsExportedTypesPerformanceCounterDescriptorDiscoveryAgentTests()
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

        protected override void OnItemsDiscovered(IEnumerable<IPerformanceCounterDescriptor> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(discoveredItems, d =>
            {
                Assert.NotNull(d);
                Assert.NotNull(d.AdapterDescriptor);
                Assert.NotNull(d.CategoryDescriptor);
                Assert.NotNull(d.Method);
                Assert.NotNull(d.Method.DeclaringType);
                Assert.NotNull(d.Method.ReflectedType);
            });

            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            var orderedItems = discoveredItems.Order().ToArray();

            var voidType = typeof(void);

            const string internalTargetMethod = "InternalTargetMethod";
            const string methodDeclaredInBaseOnly = "MethodDeclaredInBaseOnly";
            const string methodDeclaredInDerivedOnly = "MethodDeclaredInDerivedOnly";
            const string virtualMethodDecoratedInBaseOnly = "VirtualMethodDecoratedInBaseOnly";
            const string virtualMethodDecoratedInDerivedClass = "VirtualMethodDecoratedInDerivedClass";
            const string virtualMethodDecorationOvershadowed = "VirtualMethodDecorationOvershadowed";

            Assert.Collection(orderedItems,
                d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClass>(voidType, methodDeclaredInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClass, SubjectClass>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(internalTargetMethod, d.CounterName);
                    d.VerifyPublishingOptions(false, false, true)
                        .VerifySamplingOptions(Constants.MinSampleRate);
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, internalTargetMethod, false);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(methodDeclaredInDerivedOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, methodDeclaredInDerivedOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInBaseOnly, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    Assert.Null(d.RandomSeed);
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecoratedInBaseOnly);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecoratedInDerivedClass, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
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
                    d.Method.Verify<SubjectClassWithNonPublicMethods,
                        SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                }
                , d =>
                {
                    Assert.Equal(virtualMethodDecorationOvershadowed, d.CounterName);
                    d.VerifyPublishingOptions().VerifySamplingOptions();
                    d.Method.Verify<SubjectClassWithNonPublicMethods
                        , SubjectClassWithNonPublicMethods>(voidType, virtualMethodDecorationOvershadowed);
                    d.VerifyCounterAdapter<AverageTimePerformanceCounterAdapter>();
                    d.VerifyCounterCategoryAdapter();
                });
        }
    }
}
