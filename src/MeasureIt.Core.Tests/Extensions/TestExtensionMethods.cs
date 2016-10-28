using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MeasureIt
{
    using Discovery;
    using Xunit;

    internal static class TestExtensionMethods
    {
        internal static void VerifyGuid(this string s)
        {
            Guid guid;
            Assert.True(Guid.TryParse(s, out guid));
        }

        private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;

        internal static TOptions VerifyOptions<TOptions>(this TOptions options
            , BindingFlags expectedMethodBindingFlags = PublicInstance
            , bool expectedIncludeInherited = true
            , bool expectedHasRandomSeed = false
            )
            where TOptions : class, IInstrumentationDiscoveryOptions
        {
            Assert.NotNull(options);
            Assert.Equal(expectedMethodBindingFlags, options.MethodBindingAttr);
            Assert.Equal(expectedIncludeInherited, options.IncludeInherited);
            Assert.Equal(options.RandomSeed.HasValue, expectedHasRandomSeed);
            return options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReflected"></typeparam>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="method"></param>
        /// <param name="expectedName"></param>
        /// <param name="expectedPublic"></param>
        internal static void Verify<TReflected, TDeclaring, TReturn>(this MethodInfo method,
            string expectedName, bool expectedPublic = true)
        {
            Assert.Equal(expectedName, method.Name);
            Assert.Equal(expectedPublic, method.IsPublic);
            method.DeclaringType.Confirm<TDeclaring>();
            method.ReturnType.Confirm<TReflected>();
            method.ReflectedType.Confirm<TReturn>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReflected"></typeparam>
        /// <typeparam name="TDeclaring"></typeparam>
        /// <param name="method"></param>
        /// <param name="expectedReturnType"></param>
        /// <param name="expectedName"></param>
        /// <param name="expectedPublic"></param>
        internal static void Verify<TReflected, TDeclaring>(this MethodInfo method,
            Type expectedReturnType, string expectedName, bool expectedPublic = true)
        {
            // Sort out the concerns in order of appearance, from generics then from params.
            method.ReflectedType.Confirm<TReflected>();
            method.DeclaringType.Confirm<TDeclaring>();
            Assert.Equal(expectedName, method.Name);
            Assert.Equal(expectedPublic, method.IsPublic);
            method.ReturnType.Confirm(expectedReturnType);
        }

        internal static IEnumerable<IPerformanceCounterAdapterDescriptor> Order(
            this IEnumerable<IPerformanceCounterAdapterDescriptor> descriptors)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(descriptors, d =>
            {
                Assert.NotNull(d);
                Assert.NotNull(d.AdapterType);
                Assert.NotNull(d.CreationDataDescriptors);
                Assert.NotEmpty(d.CreationDataDescriptors);
            });

            // ReSharper disable once PossibleMultipleEnumeration
            return descriptors
                .OrderBy(x => x.AdapterType.FullName)
                .ThenBy(x => x.CounterName);
        }

        internal static void VerifyCounterAdapter(this IPerformanceCounterAdapterDescriptor descriptor)
        {
        }

        internal static IEnumerable<IMeasurePerformanceDescriptor> Order(
            this IEnumerable<IMeasurePerformanceDescriptor> descriptors)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(descriptors, d =>
            {
                Assert.NotNull(d);
                Assert.NotNull(d.AdapterTypes);
                Assert.NotEmpty(d.AdapterTypes);
                Assert.NotNull(d.CategoryType);
                Assert.NotNull(d.Method);
                Assert.NotNull(d.Method.ReflectedType);
                Assert.NotNull(d.Method.DeclaringType);
                Assert.NotNull(d.RootType);
            });

            /* TODO: TBD: ditto along the lines of possible less reliable ReflectedType;
             * here, the theory is that RootType is more important. */

            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            return descriptors
                .OrderBy(x => x.RootType.FullName)
                //.OrderBy(x => x.Method.ReflectedType.FullName)
                .ThenBy(x => x.Method.DeclaringType.FullName)
                .ThenBy(x => x.Method.Name)
                .ThenBy(x => x.CategoryType.FullName)
                ;
        }

        internal static IMeasurePerformanceDescriptor VerifyPublishingOptions(
            this IMeasurePerformanceDescriptor descriptor,
            bool expectedPublishCounters = true, bool expectedPublishEvent = true,
            bool expectedThrowPublishErrors = false, bool? expectedMayProceedUnabated = null)
        {
            Assert.NotNull(descriptor);
            Assert.Equal(expectedPublishCounters, descriptor.PublishCounters);
            Assert.Equal(expectedPublishEvent, descriptor.PublishEvent);
            Assert.Equal(expectedThrowPublishErrors, descriptor.ThrowPublishErrors);

            expectedMayProceedUnabated
                = expectedMayProceedUnabated
                  ?? !(expectedPublishCounters || expectedPublishEvent);

            Assert.Equal(expectedMayProceedUnabated, descriptor.MayProceedUnabated);

            return descriptor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="expectedSampleRate"></param>
        /// <param name="expectedReadOnly"></param>
        /// <param name="expectedInstanceLifetime"></param>
        internal static IMeasurePerformanceDescriptor VerifySamplingOptions(
            this IMeasurePerformanceDescriptor descriptor
            , double expectedSampleRate = Constants.MaxSampleRate
            , bool? expectedReadOnly = null
            , PerformanceCounterInstanceLifetime expectedInstanceLifetime = PerformanceCounterInstanceLifetime.Process
            )
        {
            Assert.NotNull(descriptor);
            Assert.Equal(expectedSampleRate, descriptor.SampleRate);
            Assert.Equal(expectedReadOnly, descriptor.ReadOnly);
            Assert.Equal(expectedInstanceLifetime, descriptor.InstanceLifetime);

            return descriptor;
        }

        //// TODO: TBD: refactor this as an Assert.Collection on the descriptor.AdapterTypes
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TAdapter"></typeparam>
        ///// <param name="descriptor"></param>
        ///// <param name="verify"></param>
        //internal static void VerifyCounterAdapter<TAdapter>(this IMeasurePerformanceDescriptor descriptor,
        //    Action<IPerformanceCounterAdapterDescriptor> verify = null)
        //    where TAdapter : PerformanceCounterAdapterBase<TAdapter>
        //{
        //    verify = verify ?? (d => { });
        //    descriptor.AdapterTypes.Confirm<TAdapter>();
        //    Assert.NotNull(descriptor.AdapterDescriptors);
        //    Assert.NotEmpty(descriptor.AdapterDescriptors);
        //    verify(descriptor.AdapterDescriptor);
        //}

        internal static void VerifyCounterCategoryAdapter<TCategory>(this IMeasurePerformanceDescriptor descriptor,
            PerformanceCounterCategoryType expectedCategoryType = PerformanceCounterCategoryType.MultiInstance,
            Action<IPerformanceCounterCategoryDescriptor> verify = null)
            where TCategory : PerformanceCounterCategoryAdapterBase
        {
            VerifyCounterCategoryAdapter(descriptor, typeof(TCategory), expectedCategoryType, verify);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="expectedType">Provide the expected <see cref="Type"/>. May be null under some test cases.</param>
        /// <param name="expectedCategoryType"></param>
        /// <param name="verify"></param>
        internal static void VerifyCounterCategoryAdapter(
            this IMeasurePerformanceDescriptor descriptor, Type expectedType = null,
            PerformanceCounterCategoryType expectedCategoryType = PerformanceCounterCategoryType.MultiInstance,
            Action<IPerformanceCounterCategoryDescriptor> verify = null)
        {
            verify = verify ?? (d => { });
            Assert.NotNull(descriptor);
            Assert.NotNull(descriptor.CategoryDescriptor);
            Assert.Equal(expectedCategoryType, descriptor.CategoryDescriptor.CategoryType);
            Assert.Equal(expectedType, descriptor.CategoryDescriptor.Type);
            verify(descriptor.CategoryDescriptor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAdapter"></typeparam>
        /// <param name="descriptor"></param>
        /// <param name="verify"></param>
        private static void VerifyCreationData<TAdapter>(
            this IPerformanceCounterAdapterDescriptor descriptor,
            Action<IEnumerable<ICounterCreationDataDescriptor>> verify = null)
            where TAdapter : class, IPerformanceCounterAdapter
        {
            verify = verify ?? (_ => { });
            Assert.Equal(typeof(TAdapter), descriptor.AdapterType);
            Assert.NotNull(descriptor.CreationDataDescriptors);
            Assert.NotEmpty(descriptor.CreationDataDescriptors);
            verify(descriptor.CreationDataDescriptors);
        }

        /// <summary>
        /// <see cref="AverageTimePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyAverageTimeCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;

            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<AverageTimePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageTimer, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Null(y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        , y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(averageBase, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Null(y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="CurrentConcurrentCountPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyCurrentConcurrentCountCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<CurrentConcurrentCountPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Number of requests running concurrently.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="ErrorRatePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyErrorRateCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<ErrorRatePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(rateOfCountsPerSecond, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Number of errors per second.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="LastMemberExecutionTimePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyLastMemberAccessTimeCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<LastMemberExecutionTimePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Last member execution time in milliseconds.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="MemberAccessRatePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyMemberAccessRateCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<MemberAccessRatePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(rateOfCountsPerSecond, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Number of member accesses per second.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="TotalMemberAccessesPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyTotalMemberAccessesCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<TotalMemberAccessesPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Total number of member accesses.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="MemberActivityTimerPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="descriptor"></param>
        private static void VerifyMemberActivityTimerCreationData(this IPerformanceCounterAdapterDescriptor descriptor)
        {
            const PerformanceCounterType timer = PerformanceCounterType.Timer100Ns;
            const PerformanceCounterInstanceLifetime process = PerformanceCounterInstanceLifetime.Process;

            descriptor.VerifyCreationData<MemberActivityTimerPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x,
                        y =>
                        {
                            y.Name.CanParse<string, Guid>(Guid.TryParse);
                            y.InstanceName.CanParse<string, Guid>(Guid.TryParse);
                            Assert.Equal(timer, y.CounterType);
                            Assert.Equal(process, y.InstanceLifetime);
                            Assert.Equal("Measure of member activity in nanoseconds.", y.Help);
                            Assert.Null(y.ReadOnly);
                        }
                        );
                });
        }

        internal static void Verify(this IEnumerable<IPerformanceCounterAdapterDescriptor> discoveredItems)
        {
            var orderedItems = discoveredItems.Order().ToArray();

            Assert.Collection(orderedItems,
                d =>
                {
                    Assert.Equal("average time", d.CounterName);
                    Assert.Equal(string.Empty, d.CounterHelp);
                    d.VerifyAverageTimeCreationData();
                }
                , d =>
                {
                    Assert.Equal("current concurrent count", d.CounterName);
                    Assert.Equal(string.Empty, d.CounterHelp);
                    d.VerifyCurrentConcurrentCountCreationData();
                }
                , d =>
                {
                    Assert.Equal("error rate", d.CounterName);
                    Assert.Equal("Number of errors per second (Hz).", d.CounterHelp);
                    d.VerifyErrorRateCreationData();
                }
                , d =>
                {
                    Assert.Equal("last member execution time", d.CounterName);
                    Assert.Equal(string.Empty, d.CounterHelp);
                    d.VerifyLastMemberAccessTimeCreationData();
                }
                , d =>
                {
                    Assert.Equal("member access rate", d.CounterName);
                    Assert.Equal("Number of member accesses per second (Hz).", d.CounterHelp);
                    d.VerifyMemberAccessRateCreationData();
                }
                , d =>
                {
                    Assert.Equal("member activity timer", d.CounterName);
                    Assert.Equal(string.Empty, d.CounterHelp);
                    d.VerifyMemberActivityTimerCreationData();
                }
                , d =>
                {
                    Assert.Equal("total member accesses", d.CounterName);
                    Assert.Equal("Total number of member accesses.", d.CounterHelp);
                    d.VerifyTotalMemberAccessesCreationData();
                }
                );
        }
    }
}
