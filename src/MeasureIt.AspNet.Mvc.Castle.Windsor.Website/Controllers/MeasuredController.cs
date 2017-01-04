using System.Web.Mvc;

namespace MeasureIt.AspNet.Mvc.Castle.Windsor.Controllers
{
    using Instrumentation;
    using Web.Mvc.Filters;

    public class MeasuredController : Controller
    {
        [PerformanceMeasurementFilter(
            typeof(MyPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , PublishCounters = true, PublishEvent = true, ThrowPublishErrors = true
        )]
        public ActionResult Index()
        {
            return View();
        }
    }
}
