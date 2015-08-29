using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Swetugg.Web.Migrations;

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

	    [Route("migration")]
        [RequireHttps]
        [Authorize(Roles = "Administrator")]
	    public ActionResult Migrations(string errorMsg)
	    {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
	        ViewBag.AppliedMigrations = migrator.GetDatabaseMigrations();
	        ViewBag.PendingMigrations = migrator.GetPendingMigrations();
	        ViewBag.ErrorMessage = errorMsg;

	        return View();
	    }

        [Route("migration/run")]
        [RequireHttps]
        [Authorize(Roles = "Administrator")]
        public ActionResult RunMigrations()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);

            try
            {
                migrator.Update();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Migrations", new { errorMsg = ex.Message});
            }

            return RedirectToAction("Migrations");
        }

	}
}