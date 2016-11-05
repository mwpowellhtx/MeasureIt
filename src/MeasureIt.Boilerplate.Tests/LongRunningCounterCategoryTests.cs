using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MeasureIt
{
    using Xunit;

    public class LongRunningCounterCategoryTests : PerformanceCounterCategoryTestFixtureBase
    {
        protected readonly IDictionary<PerformanceCounterType, string> NamedCounterTypes
            = new ConcurrentDictionary<PerformanceCounterType, string>();

        public LongRunningCounterCategoryTests()
        {
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            NamedCounterTypes[averageBase] = GetNewName();
            NamedCounterTypes[averageTimer] = GetNewName();
            NamedCounterTypes[rateOfCountsPerSecond] = GetNewName();
            NamedCounterTypes[numberOfItems] = GetNewName();
        }

        protected override IEnumerable<PerformanceCounterDescriptorFixture> GetPerformanceCounterDescriptors()
        {
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[averageBase], averageBase);
            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[averageTimer], averageTimer);
            yield return
                new PerformanceCounterDescriptorFixture(NamedCounterTypes[rateOfCountsPerSecond], rateOfCountsPerSecond)
                ;
            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[numberOfItems], numberOfItems);
        }

        protected override void OnVerifyDefaultPerformanceCounters()
        {
            // Do nothing for these unit tests...
        }

        protected override void OnVerifyProcessPerformanceCountersCorrect()
        {
            // Do nothing for these unit tests...
        }

        private IEnumerable<Action<PerformanceCounter>> GetCounterVerification()
        {
            return NamedCounterTypes.Select(_ => (Action<PerformanceCounter>) (c => { }));
        }

        [Theory]
        [InlineData(2000d)]
        [InlineData(5000d)]
        [InlineData(10000d)]
        public void VerifyLongRunningPerformanceCounters(double timeoutMilliseconds)
        {
            var categoryName = CategoryName;
            var instanceName = InstanceName;

            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            CreateCategory(
                c =>
                {
                    Assert.NotNull(c);
                    Assert.Equal(categoryName, c.CategoryName);
                }
                , c =>
                {
                    var counters = Specifications.Select(x => x.NewPerformanceCounter(c, instanceName)).ToArray();

                    var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                    var stopwatch = Stopwatch.StartNew();

                    // These are no doubt clumsy usage examples, but they should "work" at least by incrementing.
                    do
                    {
                        TimeSpan.FromMilliseconds(250d).Sleep();
                        counters.Single(x => x.CounterType == averageBase).Increment();
                        counters.Single(x => x.CounterType == rateOfCountsPerSecond).Increment();
                        counters.Single(x => x.CounterType == numberOfItems).Increment();
                    } while (stopwatch.Elapsed < timeout);

                    return counters;
                }, GetCounterVerification().ToArray()
                );
        }
    }

    internal static class LongRunningExtensionMethods
    {
        internal static void Sleep(this TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
        }
    }
}
