using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    using Xunit;

    public abstract class PerformanceCounterCategoryTestFixtureBase : TestFixtureBase
    {
        protected class PerformanceCounterDescriptorFixture
        {
            internal string Name { get; private set; }

            internal string Help { get; private set; }

            internal PerformanceCounterType CounterType { get; private set; }

            internal PerformanceCounterInstanceLifetime InstanceLifetime { get; private set; }

            internal bool? ReadOnly { get; private set; }

            internal PerformanceCounterDescriptorFixture(string name, PerformanceCounterType counterType)
                : this(name, string.Empty, counterType)
            {
            }

            internal PerformanceCounterDescriptorFixture(string name, PerformanceCounterType counterType, bool readOnly)
                : this(name, string.Empty, counterType, readOnly)
            {
            }

            internal PerformanceCounterDescriptorFixture(string name, string help, PerformanceCounterType counterType,
                bool? readOnly = null)
            {
                Name = name;
                Help = help;
                CounterType = counterType;
                ReadOnly = readOnly;
                InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
            }

            internal CounterCreationData NewCreationData()
            {
                var datum = new CounterCreationData(Name, Help, CounterType);
                Assert.Equal(Name, datum.CounterName);
                Assert.NotNull(datum.CounterHelp);
                Assert.Equal(CounterType, datum.CounterType);
                return datum;
            }

            internal PerformanceCounter NewPerformanceCounter(PerformanceCounterCategory category, string instanceName)
            {
                const string localhost = ".";
                var counter = new PerformanceCounter();
                counter.CategoryName = category.CategoryName;
                counter.CounterName = Name;
                counter.ReadOnly = ReadOnly ?? false;
                counter.InstanceLifetime = InstanceLifetime;
                counter.MachineName = localhost;
                counter.InstanceName = instanceName;
                return counter;
            }
        }

        private readonly Lazy<string> _lazyCategoryName;

        private readonly Lazy<string> _lazyCategoryHelp;

        private readonly Lazy<string> _lazyInstanceName;

        protected string CategoryName
        {
            get { return _lazyCategoryName.Value; }
        }

        protected string CategoryHelp
        {
            get { return _lazyCategoryHelp.Value; }
        }

        protected string InstanceName
        {
            get { return _lazyInstanceName.Value; }
        }

        protected static string GetNewName()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Help must have some value, even <see cref="string.Empty"/>.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCategoryHelp()
        {
            return string.Empty;
        }

        private readonly Lazy<IEnumerable<PerformanceCounterDescriptorFixture>> _lazySpecifications;

        protected IEnumerable<PerformanceCounterDescriptorFixture> Specifications
        {
            get { return _lazySpecifications.Value; }
        }

        private IEnumerable<PerformanceCounterDescriptorFixture> OrderedSpecifications
        {
            get { return Specifications.OrderBy(x => x.CounterType).ThenBy(x => x.Name); }
        }

        protected virtual IEnumerable<CounterCreationData> OnCreationData(IEnumerable<CounterCreationData> items)
        {
            return items;
        }

        protected IEnumerable<CounterCreationData> CreationData
        {
            get { return OnCreationData(OrderedSpecifications.Select(s => s.NewCreationData())); }
        }

        protected abstract IEnumerable<PerformanceCounterDescriptorFixture> GetPerformanceCounterDescriptors();

        protected PerformanceCounterCategoryTestFixtureBase()
        {
            _lazyCategoryName = new Lazy<string>(GetNewName);
            _lazyCategoryHelp = new Lazy<string>(GetCategoryHelp);
            _lazyInstanceName = new Lazy<string>(GetNewName);
            _lazySpecifications = new Lazy<IEnumerable<PerformanceCounterDescriptorFixture>>(
                GetPerformanceCounterDescriptors);
        }

        protected void CreateCategory(Action<PerformanceCounterCategory> action = null,
            Func<PerformanceCounterCategory, IEnumerable<PerformanceCounter>> getCounters = null,
            params Action<PerformanceCounter>[] verifyCounters)
        {
            var expectedHelp = string.IsNullOrEmpty(CategoryHelp) ? "Help not available." : CategoryHelp;

            action = action ?? (c =>
            {
                Assert.NotNull(c);
                Assert.Equal(CategoryName, c.CategoryName);
                Assert.Equal(expectedHelp, c.CategoryHelp);
                Assert.Equal(PerformanceCounterCategoryType.MultiInstance, c.CategoryType);
            });

            getCounters = getCounters ?? (c => c.GetCounters());

            try
            {
                const PerformanceCounterCategoryType categoryType = PerformanceCounterCategoryType.MultiInstance;

                var category = PerformanceCounterCategory.Create(CategoryName, CategoryHelp, categoryType,
                    new CounterCreationDataCollection(CreationData.ToArray()));

                // This is necessary in order to clean up internal resources, for whatever reason.
                // See: http://blog.dezfowler.com/2007/08/net-performance-counter-problems.html
                PerformanceCounter.CloseSharedResources();

                action(category);

                var counters = getCounters(category)
                    .OrderBy(x => x.CounterType)
                    .ThenBy(x => x.CounterName).ToArray();

                Assert.Collection(counters, verifyCounters);
            }
            catch (Exception)
            {
                DeleteCategory(CategoryName);
                throw;
            }
        }

        [Fact]
        public void VerifyThatCounterCreationDataCorrect()
        {
            Assert.All(CreationData, Assert.NotNull);
        }

        protected abstract void OnVerifyDefaultPerformanceCounters();

        [Fact]
        public void VerifyThatDefaultPerformanceCountersCorrect()
        {
            OnVerifyDefaultPerformanceCounters();
        }

        protected abstract void OnVerifyProcessPerformanceCountersCorrect();

        [Fact]
        public void VerifyThatProcessPerformanceCountersCorrect()
        {
            OnVerifyProcessPerformanceCountersCorrect();
        }

        private static void DeleteCategory(string categoryName)
        {
            if (!PerformanceCounterCategory.Exists(categoryName)) return;
            PerformanceCounterCategory.Delete(categoryName);
        }

        protected virtual void VerifyCreatedCategories(
            Func<PerformanceCounterCategory, IEnumerable<PerformanceCounter>> getCounters
            , Action<PerformanceCounter> verifyAll = null, params Action<PerformanceCounter>[] verifyCollection)
        {
            verifyAll = verifyAll ?? (c => { });

            CreateCategory(x =>
            {
                var counters = getCounters(x).OrderBy(c => c.CounterType).ToArray();

                Assert.All(counters, verifyAll);

                Assert.Collection(counters, verifyCollection);
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                DeleteCategory(CategoryName);
            }

            base.Dispose(disposing);
        }
    }
}
