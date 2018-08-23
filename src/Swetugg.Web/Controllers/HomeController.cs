﻿using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Swetugg.Web.Controllers
{
    public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return RedirectToAction("Index", "Conference", new { Area = "Swetugg2019"});
		}

	    [Route("techdays")]
	    public ActionResult TechDays()
	    {
	        var ticketUrl = ConfigurationManager.AppSettings["Ticket_Url_Full"];

	        return Redirect(ticketUrl);
	    }

        [Route("now")]
        public ActionResult Now()
        {
            return RedirectToAction("Now", "Conference", new { Area = "Swetugg2019" });
        }

        [Route("code-of-conduct")]
        public ActionResult CodeOfConduct()
        {
            return RedirectToAction("CodeOfConduct", "Conference", new { Area = "Swetugg2019" });
        }

        [Route("start")]
	    public ActionResult Start()
        {
            return View();
        }

	}
}