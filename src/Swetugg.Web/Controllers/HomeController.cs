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
			return RedirectToAction("Index", "Conference", new { Area = "Swetugg2016"});
		}
        
        [Route("start")]
	    public ActionResult Start()
        {
            return View();
        }

	}
}