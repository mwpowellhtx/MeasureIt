using System.Diagnostics;

namespace MeasureIt.Castle.Windsor.AspNet.WebApi
{
    using Xunit;

    /// <summary>
    /// Handle any potential bookkeeping tasks associated with orphaned or abandaoned
    /// categories or other resources.
    /// </summary>
    public class BookkeepingTests : DisposableTestFixtureBase
    {
        [Theory
        , InlineData("WebApiPerformanceCategoryAdapter")
        ]
        public void VerifyApiValuesCorrect(string categoryName)
        {
            if (!PerformanceCounterCategory.Exists(categoryName)) return;
            PerformanceCounterCategory.Delete(categoryName);
        }
    }
}
