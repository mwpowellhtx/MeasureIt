using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MeasureIt
{
    using Xunit;
    using NamedCounterTypeTuple = Tuple<PerformanceCounterType, string>;

    public class LongRunningCounterCategoryTests : PerformanceCounterCategoryTestFixtureBase
    {
        protected readonly IList<NamedCounterTypeTuple> NamedCounterTypes = new List<NamedCounterTypeTuple>();

        public LongRunningCounterCategoryTests()
        {
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            NamedCounterTypes.Add(averageTimer, GetNewName());
            NamedCounterTypes.Add(averageBase, GetNewName());
            NamedCounterTypes.Add(rateOfCountsPerSecond, GetNewName());
            NamedCounterTypes.Add(numberOfItems, GetNewName());
        }

        protected override IEnumerable<PerformanceCounterDescriptorFixture> GetPerformanceCounterDescriptors()
        {
            return NamedCounterTypes.Select(tuple => new PerformanceCounterDescriptorFixture(tuple.Item2, tuple.Item1));
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

        internal static TCollection Add<TCollection>(this TCollection collection,
            PerformanceCounterType counterType, string name)
            where TCollection : IList<NamedCounterTypeTuple>
        {
            collection.Add(Tuple.Create(counterType, name));
            return collection;
        }
    }
}
