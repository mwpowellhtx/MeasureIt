using System.Web.Mvc;

namespace MeasureIt.Web.Mvc.Controllers
{
    // TODO: TBD: whether to establish abstract base classes... is an abstract method one of the use cases under test?
    public class HomeController : Controller
    {
        // TODO: TBD: instrument the controllers here...
        public ActionResult Index()
        {
            return View();
        }

        // TODO: TBD: instrument the controllers here...
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        // TODO: TBD: instrument the controllers here...
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}