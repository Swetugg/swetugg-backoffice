using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace Swetugg.Web.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
		public Microsoft.AspNetCore.Mvc.ActionResult Index()
		{
			return RedirectToAction("Index", "Conference", new { Area = "gbg2022"});
		}

	    [Route("techdays")]
	    public Microsoft.AspNetCore.Mvc.ActionResult TechDays()
	    {
	        var ticketUrl = ConfigurationManager.AppSettings["Ticket_Url_Full"];

	        return Redirect(ticketUrl);
	    }

        [Route("now")]
        public Microsoft.AspNetCore.Mvc.ActionResult Now()
        {
            return RedirectToAction("Now", "Conference", new { Area = "gbg2022" });
        }

        [Route("code-of-conduct")]
        public Microsoft.AspNetCore.Mvc.ActionResult CodeOfConduct()
        {
            return RedirectToAction("CodeOfConduct", "Conference", new { Area = "gbg2022" });
        }

        [Route("start")]
	    public Microsoft.AspNetCore.Mvc.ActionResult Start()
        {
            return View();
        }

	}
}