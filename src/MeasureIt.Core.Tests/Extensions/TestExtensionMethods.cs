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
            , Action<TOptions> verify = null
            )
            where TOptions : class, IInstrumentationDiscoveryOptions
        {
            verify = verify ?? (o => { });
            Assert.NotNull(options);
            Assert.Equal(expectedMethodBindingFlags, options.MethodBindingAttr);
            Assert.Equal(expectedIncludeInherited, options.IncludeInherited);
            Assert.Equal(options.RandomSeed.HasValue, expectedHasRandomSeed);
            Assert.NotNull(options.Assemblies);
            verify(options);
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

        internal static IEnumerable<IPerformanceCounterAdapter> Order(
            this IEnumerable<IPerformanceCounterAdapter> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(discoveredItems
                , d =>
                {
                    Assert.NotNull(d);
                    Assert.NotNull(d.CreationData);
                    Assert.NotEmpty(d.CreationData);
                }
                );

            // ReSharper disable once PossibleMultipleEnumeration
            return discoveredItems
                .OrderBy(x => x.GetType().FullName)
                ;
        }

        internal static IEnumerable<IPerformanceCounterCategoryAdapter> Order(
            this IEnumerable<IPerformanceCounterCategoryAdapter> categories)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(categories
                , d =>
                {
                    Assert.NotNull(d);
                    Assert.NotNull(d.Name);
                    Assert.NotEmpty(d.Name);
                    // May be empty here; depending on which discovery agents has "seen" them yet.
                    Assert.NotNull(d.CreationData);
                    Assert.NotNull(d.Measurements);
                });

            // ReSharper disable once PossibleMultipleEnumeration
            return categories
                .OrderBy(x => x.CategoryType)
                .ThenBy(x => x.Name);
        }

        internal static void VerifyCounterAdapter(this IPerformanceCounterAdapter adapter)
        {
        }

        internal static IEnumerable<IPerformanceMeasurementDescriptor> Order(
            this IEnumerable<IPerformanceMeasurementDescriptor> discoveredItems)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(discoveredItems
                , d =>
                {
                    Assert.NotNull(d);
                    Assert.NotNull(d.AdapterTypes);
                    Assert.NotEmpty(d.AdapterTypes);
                    Assert.NotNull(d.CategoryType);
                    Assert.NotNull(d.Method);
                    Assert.NotNull(d.Method.ReflectedType);
                    Assert.NotNull(d.Method.DeclaringType);
                    Assert.NotNull(d.RootType);
                }
                );

            /* TODO: TBD: ditto along the lines of possible less reliable ReflectedType;
             * here, the theory is that RootType is more important. */

            // ReSharper disable once PossibleMultipleEnumeration, PossibleNullReferenceException
            return discoveredItems
                .OrderBy(x => x.RootType.FullName)
                //.OrderBy(x => x.Method.ReflectedType.FullName)
                .ThenBy(x => x.Method.DeclaringType.FullName)
                .ThenBy(x => x.Method.Name)
                .ThenBy(x => x.CategoryType.FullName)
                ;
        }

        internal static IEnumerable<ICounterCreationDataDescriptor> Order(
            this IEnumerable<ICounterCreationDataDescriptor> descriptors
            )
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(descriptors, d => Assert.NotNull(d.Name));

            // ReSharper disable once PossibleMultipleEnumeration
            return descriptors
                .OrderBy(x => x.CounterType)
                .ThenBy(x => x.Help)
                ;
        }

        internal static IPerformanceMeasurementDescriptor VerifyPublishingOptions(
            this IPerformanceMeasurementDescriptor descriptor,
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
        internal static IPerformanceMeasurementDescriptor VerifySamplingOptions(
            this IPerformanceMeasurementDescriptor descriptor
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

        internal static void VerifyCounterCategoryAdapter<TCategory>(this IPerformanceMeasurementDescriptor descriptor,
            PerformanceCounterCategoryType expectedCategoryType = PerformanceCounterCategoryType.MultiInstance,
            Action<IPerformanceCounterCategoryAdapter> verify = null)
            where TCategory : PerformanceCounterCategoryAdapterBase
        {
            VerifyCounterCategoryAdapter(descriptor, typeof(TCategory), expectedCategoryType, verify);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="expectedType"></param>
        /// <param name="expectedCategoryType"></param>
        /// <param name="verify"></param>
        internal static void VerifyCounterCategoryAdapter(
            this IPerformanceMeasurementDescriptor descriptor,
            Type expectedType = null,
            PerformanceCounterCategoryType expectedCategoryType = PerformanceCounterCategoryType.MultiInstance,
            Action<IPerformanceCounterCategoryAdapter> verify = null)
        {
            verify = verify ?? (d => { });

            Assert.NotNull(descriptor);

            if (expectedType != null)
            {
                Assert.NotNull(descriptor.CategoryAdapter);
                descriptor.CategoryAdapter.GetType().Confirm(expectedType);
                Assert.Equal(expectedCategoryType, descriptor.CategoryAdapter.CategoryType);
            }
            else
            {
                Assert.Null(descriptor.CategoryAdapter);
            }

            verify(descriptor.CategoryAdapter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAdapter"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="verify"></param>
        private static void VerifyCreationData<TAdapter>(
            this IPerformanceCounterAdapter adapter,
            Action<IEnumerable<ICounterCreationDataDescriptor>> verify = null)
            where TAdapter : class, IPerformanceCounterAdapter
        {
            verify = verify ?? (_ => { });
            Assert.IsType<TAdapter>(adapter);
            Assert.NotNull(adapter.CreationData);
            Assert.NotEmpty(adapter.CreationData);
            var orderedData = adapter.CreationData.Order().ToArray();
            verify(orderedData);
        }

        /// <summary>
        /// <see cref="AverageTimePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyAverageTimeCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;

            adapter.VerifyCreationData<AverageTimePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(averageTimer, y.CounterType);
                            Assert.NotNull(y.Help);
                            Assert.Empty(y.Help);
                        }
                        , y =>
                        {
                            Assert.EndsWith("Base", y.Name);
                            Assert.Equal(averageBase, y.CounterType);
                            Assert.NotNull(y.Help);
                            Assert.Empty(y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="CurrentConcurrentCountPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyCurrentConcurrentCountCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            adapter.VerifyCreationData<CurrentConcurrentCountPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal("Number of requests running concurrently.", y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="ErrorRatePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyErrorRateCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;

            adapter.VerifyCreationData<ErrorRatePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(rateOfCountsPerSecond, y.CounterType);
                            Assert.Equal("Number of errors per second.", y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="LastMemberExecutionTimePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyLastMemberAccessTimeCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            adapter.VerifyCreationData<LastMemberExecutionTimePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal("Last member execution time in milliseconds.", y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="MemberAccessRatePerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyMemberAccessRateCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;

            adapter.VerifyCreationData<MemberAccessRatePerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(rateOfCountsPerSecond, y.CounterType);
                            Assert.Equal("Number of member accesses per second.", y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="TotalMemberAccessesPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyTotalMemberAccessesCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            adapter.VerifyCreationData<TotalMemberAccessesPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(numberOfItems, y.CounterType);
                            Assert.Equal("Total number of member accesses.", y.Help);
                        }
                        );
                });
        }

        /// <summary>
        /// <see cref="MemberActivityTimerPerformanceCounterAdapter"/> verification is by design.
        /// </summary>
        /// <param name="adapter"></param>
        private static void VerifyMemberActivityTimerCreationData(this IPerformanceCounterAdapter adapter)
        {
            const PerformanceCounterType timer = PerformanceCounterType.Timer100Ns;

            adapter.VerifyCreationData<MemberActivityTimerPerformanceCounterAdapter>(
                x =>
                {
                    Assert.Collection(x
                        , y =>
                        {
                            Assert.Equal(timer, y.CounterType);
                            Assert.Equal("Measure of member activity in nanoseconds.", y.Help);
                        }
                        );
                });
        }

        internal static void Verify(this IEnumerable<IPerformanceCounterAdapter> discoveredItems)
        {
            var orderedItems = discoveredItems.Order().ToArray();

            Assert.Collection(orderedItems
                , d =>
                {
                    d.VerifyAverageTimeCreationData();
                }
                , d =>
                {
                    d.VerifyCurrentConcurrentCountCreationData();
                }
                , d =>
                {
                    d.VerifyErrorRateCreationData();
                }
                , d =>
                {
                    d.VerifyLastMemberAccessTimeCreationData();
                }
                , d =>
                {
                    d.VerifyMemberAccessRateCreationData();
                }
                , d =>
                {
                    d.VerifyMemberActivityTimerCreationData();
                }
                , d =>
                {
                    d.VerifyTotalMemberAccessesCreationData();
                }
                );
        }

        internal static TService VerifyDiscoveryService<TService>(this TService service, Action<TService> verify = null)
            where TService : IInstrumentationDiscoveryService
        {
            verify = verify ?? (s => { });

            Assert.NotNull(service);

            verify(service);

            return service;
        }

        internal static TService VerifyDiscover<TService>(this TService service)
            where TService : IInstrumentationDiscoveryService
        {
            Assert.True(service.IsPending);

            service.Discover();

            Assert.False(service.IsPending);

            return service;
        }
    }
}
