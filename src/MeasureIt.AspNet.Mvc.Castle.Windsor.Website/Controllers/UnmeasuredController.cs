using System.Web.Mvc;

namespace MeasureIt.AspNet.Mvc.Castle.Windsor.Controllers
{
    public class UnmeasuredController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
