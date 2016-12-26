using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MeasureIt
{
    using Xunit;

    public class CleanUpPerformanceCounterTests : TestFixtureBase
    {
        /// <summary>
        /// Returns whether <paramref name="s"/> CanParse as a <see cref="Guid"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool CanParse(string s)
        {
            Guid parsed;
            // We do not care what the value was, just whether it Could be Parsed.
            return Guid.TryParse(s, out parsed);
        }

        private readonly IEnumerable<PerformanceCounterCategory> _categories;

        public CleanUpPerformanceCounterTests()
        {
            /* Operating under the assumption that test categories/counters are creating utilizing
             * a Guid-based naming convention. Hopefully that does not step on too many toes. */

            _categories = PerformanceCounterCategory.GetCategories()
                .Where(x => CanParse(x.CategoryName)).ToArray();
        }

        [Fact]
        public void TearDown()
        {
            // Just pass this one, for sake of fulfilling the "test".
            Assert.True(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                // Delete all of the the Existing Categories.
                foreach (var categoryName in _categories.Select(x => x.CategoryName)
                    .Where(PerformanceCounterCategory.Exists))
                {
                    PerformanceCounterCategory.Delete(categoryName);
                }
            }

            base.Dispose(disposing);
        }
    }
}
