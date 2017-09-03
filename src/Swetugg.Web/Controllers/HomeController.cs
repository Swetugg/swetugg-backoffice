using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Swetugg.Web.Controllers
{
    public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Conference", new { Area = "Swetugg2018"});
		}

        [Route("now")]
        public ActionResult Now()
        {
            return RedirectToAction("Now", "Conference", new { Area = "Swetugg2018" });
        }

        [Route("code-of-conduct")]
        public ActionResult CodeOfConduct()
        {
            return RedirectToAction("CodeOfConduct", "Conference", new { Area = "Swetugg2018" });
        }

        [Route("start")]
	    public ActionResult Start()
        {
            return View();
        }

	}
}