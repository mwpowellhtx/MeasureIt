using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MeasureIt.Web.Http.Controllers
{
    using Counters;
    using Filters;

    public abstract class MeasuredControllerBase : ApiController
    {
        [PerformanceMeasurementFilter(
            typeof(WebApiPerformanceCounterCategoryAdapter)
            , typeof(CurrentConcurrentCountPerformanceCounterAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , PublishCounters = true, PublishEvent = true, ThrowPublishErrors = true
            )]
        public IEnumerable<int> Get()
        {
            return OddValues.ToArray();
        }

        [ActionName("verify")]
        [PerformanceMeasurementFilter(
            typeof(WebApiPerformanceCounterCategoryAdapter)
            , typeof(CurrentConcurrentCountPerformanceCounterAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , PublishCounters = true, PublishEvent = true, ThrowPublishErrors = true
            )]
        public IEnumerable<int> Get(int value)
        {
            return OddValues.Where(x => x == value).ToArray();
        }

        internal static IEnumerable<int> OddValues
        {
            get
            {
                yield return 1;
                yield return 3;
                yield return 5;
                yield return 7;
                yield return 9;
            }
        }
    }
}
