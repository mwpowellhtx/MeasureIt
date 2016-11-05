using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    using Xunit;

    public class SimpleCounterCategoryTests : PerformanceCounterCategoryTestFixtureBase
    {
        protected readonly IDictionary<PerformanceCounterType, string> NamedCounterTypes
            = new ConcurrentDictionary<PerformanceCounterType, string>();

        public SimpleCounterCategoryTests()
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

        /// <summary>
        /// Since apparently PerformanceCounterType cannot be accessed, perhaps we can verify this.
        /// </summary>
        /// <param name="expectedInstancLifetime"></param>
        /// <param name="expectedReadOnly"></param>
        /// <param name="expectedInstanceName"></param>
        protected IEnumerable<Action<PerformanceCounter>> GetCounterVerification(
            PerformanceCounterInstanceLifetime expectedInstancLifetime,
            bool expectedReadOnly = false, string expectedInstanceName = null)
        {
            expectedInstanceName = expectedInstanceName ?? string.Empty;

            return NamedCounterTypes
                .OrderBy(p => p.Key)
                .ThenBy(p => p.Value)
                .Select(p => (Action<PerformanceCounter>) (c =>
                {
                    Assert.Equal(p.Key, c.CounterType);
                    Assert.Equal(p.Value, c.CounterName);
                    Assert.Equal(expectedReadOnly, c.ReadOnly);
                    Assert.Equal(expectedInstancLifetime, c.InstanceLifetime);
                    Assert.Equal(expectedInstanceName, c.InstanceName);
                }));
        }

        protected override IEnumerable<PerformanceCounterDescriptorFixture> GetPerformanceCounterDescriptors()
        {
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;

            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[averageBase], averageBase);
            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[averageTimer], averageTimer);
            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[rateOfCountsPerSecond], rateOfCountsPerSecond);
            yield return new PerformanceCounterDescriptorFixture(NamedCounterTypes[numberOfItems], numberOfItems);
        }

        protected override IEnumerable<CounterCreationData> OnCreationData(IEnumerable<CounterCreationData> items)
        {
            const PerformanceCounterType numberOfItems = PerformanceCounterType.NumberOfItems64;
            const PerformanceCounterType rateOfCountsPerSecond = PerformanceCounterType.RateOfCountsPerSecond64;
            const PerformanceCounterType averageTimer = PerformanceCounterType.AverageTimer32;
            const PerformanceCounterType averageBase = PerformanceCounterType.AverageBase;

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(items, x => Assert.NotNull(x.CounterHelp));

            // ReSharper disable once PossibleMultipleEnumeration
            Assert.Collection(items
                , x =>
                {
                    Assert.Equal(numberOfItems, x.CounterType);
                    Assert.Equal(NamedCounterTypes[numberOfItems], x.CounterName);
                }
                , x =>
                {
                    Assert.Equal(rateOfCountsPerSecond, x.CounterType);
                    Assert.Equal(NamedCounterTypes[rateOfCountsPerSecond], x.CounterName);
                }
                , x =>
                {
                    Assert.Equal(averageTimer, x.CounterType);
                    Assert.Equal(NamedCounterTypes[averageTimer], x.CounterName);
                }
                , x =>
                {
                    Assert.Equal(averageBase, x.CounterType);
                    Assert.Equal(NamedCounterTypes[averageBase], x.CounterName);
                }
                );

            // ReSharper disable once PossibleMultipleEnumeration
            return base.OnCreationData(items);
        }

        protected override void OnVerifyDefaultPerformanceCounters()
        {
            CreateCategory(
                c =>
                {
                    Assert.NotNull(c);
                    Assert.Equal(CategoryName, c.CategoryName);
                }
                , c =>
                {
                    var counters = c.GetCounters();
                    Assert.NotNull(counters);
                    Assert.Equal(4, counters.Length);
                    Assert.All(counters, x => Assert.Equal(x.CounterName, x.CounterHelp));
                    return counters;
                }
                , GetCounterVerification(PerformanceCounterInstanceLifetime.Global, true).ToArray()
                );
        }

        protected override void OnVerifyProcessPerformanceCountersCorrect()
        {
            var instanceName = InstanceName;

            CreateCategory(
                c =>
                {
                    Assert.NotNull(c);
                    Assert.Equal(CategoryName, c.CategoryName);
                }
                , c =>
                {
                    c.ReadCategory();
                    var counters = Specifications.Select(x => x.NewPerformanceCounter(c, instanceName)).ToArray();
                    Assert.NotNull(counters);
                    Assert.Equal(4, counters.Length);
                    return counters;
                }
                , GetCounterVerification(PerformanceCounterInstanceLifetime.Process, false, instanceName).ToArray()
                );
        }

        // TODO: TBD: next up: look into actually incrementing some counters and monitoring them...
    }
}
