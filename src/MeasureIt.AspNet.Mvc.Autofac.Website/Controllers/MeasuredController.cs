using System.Web.Mvc;

namespace MeasureIt.AspNet.Mvc.Autofac.Controllers
{
    using Instrumentation;
    using Web.Mvc.Filters;
    using static Discovery.MeasurementBoundary;

    public class MeasuredController : Controller
    {
        [PerformanceMeasurementFilter(
            typeof(MyPerformanceCounterCategoryAdapter)
            , typeof(AverageTimePerformanceCounterAdapter)
            , typeof(TotalMemberAccessesPerformanceCounterAdapter)
            , PublishCounters = true, PublishEvent = true, ThrowPublishErrors = true
            , Boundary = new[] {BeginAction, EndAction}
        )]
        public ActionResult Index()
        {
            return View();
        }
    }
}
