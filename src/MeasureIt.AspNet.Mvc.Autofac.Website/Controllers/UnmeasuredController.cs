using System.Web.Mvc;

namespace MeasureIt.AspNet.Mvc.Autofac.Controllers
{
    public class UnmeasuredController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
