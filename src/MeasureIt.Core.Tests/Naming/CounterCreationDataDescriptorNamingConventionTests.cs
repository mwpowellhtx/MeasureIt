using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt.Naming
{
    using Xunit;

    public class CounterCreationDataDescriptorNamingConventionTests
        : NamingConventionTestFixtureBase<ICounterCreationDataDescriptor>
    {
        private class TestOptions : ITestOptions
        {
            internal string Suffix { get; set; }

            internal PerformanceCounterType CounterType { get; private set; }

            internal TestOptions(string suffix,
                PerformanceCounterType counterType = default(PerformanceCounterType))
            {
                Suffix = suffix;
                CounterType = counterType;
            }

            internal static TestOptions Default()
            {
                return new TestOptions(null, default(PerformanceCounterType));
            }
        }

        private TestOptions Options { get; set; }

        private ICounterCreationDataDescriptor CreateSubject()
        {
            var opts = Options ?? TestOptions.Default();

            return base.CreateSubject(opts, o =>
            {
                var suffix = o.Suffix;

                var descriptor = string.IsNullOrEmpty(suffix)
                    ? new CounterCreationDataDescriptor {CounterType = o.CounterType}
                    : new CounterCreationDataDescriptor(suffix) {CounterType = o.CounterType};

                return descriptor;
            });
        }

                //        yield return '.';
                //yield return '/';
                //yield return '\\';
                //yield return ',';
                //yield return ';';
                //yield return ':';
                //yield return '?';
                //yield return '&';
                //yield return '#';
                //yield return '%';
                //yield return '*';
                //yield return '|';

        private static string AppendBaseSuffix(string suffix, PerformanceCounterType counterType)
        {
            string @base;
            if (counterType.TryGetBaseSuffix(out @base))
                suffix = string.Join(".", new[] {suffix, @base}.Where(s => !string.IsNullOrEmpty(s)));
            return suffix ?? string.Empty;
        }

        [Theory
        , InlineData("This.Is.A.Path", "This.Is.A.Path")
        , InlineData("This/Is/A/Path", "This.Is.A.Path")
        , InlineData("This\\Is\\A\\Path", "This.Is.A.Path")
        ]
        public void VerifyCorrectNameWithDifferentSuffixFormats(string suffix, string expectedName)
        {
            Options = new TestOptions(suffix);

            var subject = CreateSubject();

            Assert.Equal(subject.Name, expectedName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="counterType"></param>
        [Theory, CombinatorialData]
        public void VerifyCorrectSuffixWithAnySuffixAndCounterType(
            [SuffixValues] string suffix
            , [PerformanceCounterTypeValuesAttribute] PerformanceCounterType counterType
            )
        {
            Options = new TestOptions(suffix, counterType);

            var subject = CreateSubject();

            var appended = AppendBaseSuffix(suffix, counterType);

            Assert.Equal(suffix, subject.Suffix);
            Assert.Equal(appended, subject.Name);
        }

        private class SuffixValuesAttribute : CombinatorialValuesAttribute
        {
            private static readonly object[] InitialValues;

            static SuffixValuesAttribute()
            {
                const string defaultValue = (string) null;
                InitialValues = new object[] {defaultValue, "Theoretical"};
            }

            internal SuffixValuesAttribute()
                : base(InitialValues)
            {
            }
        }

        private class PerformanceCounterTypeValuesAttribute : CombinatorialValuesAttribute
        {
            private static readonly object[] InitialValues;

            private static IEnumerable<T> GetInitialValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }

            static PerformanceCounterTypeValuesAttribute()
            {
                InitialValues = GetInitialValues<PerformanceCounterType>().Cast<object>().ToArray();
            }

            internal PerformanceCounterTypeValuesAttribute()
                : base(InitialValues)
            {
            }
        }
    }
}
