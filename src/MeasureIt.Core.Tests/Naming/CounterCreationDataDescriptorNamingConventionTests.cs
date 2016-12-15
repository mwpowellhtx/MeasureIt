namespace MeasureIt.Naming
{
    using Xunit;

    public class CounterCreationDataDescriptorNamingConventionTests
        : NamingConventionTestFixtureBase<ICounterCreationDataDescriptor>
    {
        //private class TestOptions : ITestOptions
        //{
        //    internal PerformanceCounterType CounterType { get; private set; }
        //    internal TestOptions(PerformanceCounterType counterType = default(PerformanceCounterType))
        //    {
        //        CounterType = counterType;
        //    }
        //    internal static TestOptions Default()
        //    {
        //        return new TestOptions();
        //    }
        //}

        //private TestOptions Options { get; set; }

        //private ICounterCreationDataDescriptor CreateSubject()
        //{
        //    var opts = Options ?? TestOptions.Default();
        //    return base.CreateSubject(opts, o => new CounterCreationDataDescriptor {CounterType = o.CounterType});
        //}

        //private static string AppendBaseSuffix(PerformanceCounterType counterType)
        //{
        //    string @base;
        //    if (counterType.TryGetBaseSuffix(out @base))
        //        suffix = string.Join(".", new[] {suffix, @base}.Where(s => !string.IsNullOrEmpty(s)));
        //    return suffix ?? string.Empty;
        //}

        //// TODO: TBD: the pretense for these unit tests is simply not there any longer...
        //[Theory
        //, InlineData("This.Is.A.Path", "This.Is.A.Path")
        //, InlineData("This/Is/A/Path", "This.Is.A.Path")
        //, InlineData("This\\Is\\A\\Path", "This.Is.A.Path")
        //]
        //public void VerifyCorrectNameWithDifferentSuffixFormats(string suffix, string expectedName)
        //{
        //    Options = new TestOptions(suffix);
        //    var subject = CreateSubject();
        //    Assert.Equal(subject.Name, expectedName);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="suffix"></param>
        ///// <param name="counterType"></param>
        //[Theory, CombinatorialData]
        //public void VerifyCorrectSuffixWithAnySuffixAndCounterType(
        //    [PerformanceCounterTypeValuesAttribute] PerformanceCounterType counterType
        //    )
        //{
        //    Options = new TestOptions(counterType);
        //    var subject = CreateSubject();
        //    var appended = AppendBaseSuffix(counterType);
        //    Assert.Equal(appended, subject.Name);
        //}

        //private class SuffixValuesAttribute : CombinatorialValuesAttribute
        //{
        //    private static readonly object[] InitialValues;
        //    static SuffixValuesAttribute()
        //    {
        //        const string defaultValue = (string) null;
        //        InitialValues = new object[] {defaultValue, "Theoretical"};
        //    }
        //    internal SuffixValuesAttribute()
        //        : base(InitialValues)
        //    {
        //    }
        //}

        //private class PerformanceCounterTypeValuesAttribute : CombinatorialValuesAttribute
        //{
        //    private static readonly object[] InitialValues;
        //    private static IEnumerable<T> GetInitialValues<T>()
        //    {
        //        return Enum.GetValues(typeof(T)).Cast<T>();
        //    }
        //    static PerformanceCounterTypeValuesAttribute()
        //    {
        //        InitialValues = GetInitialValues<PerformanceCounterType>().Cast<object>().ToArray();
        //    }
        //    internal PerformanceCounterTypeValuesAttribute()
        //        : base(InitialValues)
        //    {
        //    }
        //}

        [Fact]
        public void VerifyDefaults()
        {
            /* TODO: TBD: this is obviously a place holder; will want to establish
             * a set of sensible tests for purposes of validating naming strategies... */
            Assert.True(true);
        }
    }
}
