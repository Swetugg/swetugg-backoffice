using System;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Migrations;

namespace Swetugg.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    [RequireHttps]
    public class DatabaseController : Microsoft.AspNetCore.Mvc.Controller
    {

        [Route("backups")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Backups()
        {
            return View();
        }

        [Route("migration")]
        public Microsoft.AspNetCore.Mvc.ActionResult Migrations(string errorMsg)
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            ViewBag.AppliedMigrations = migrator.GetDatabaseMigrations();
            ViewBag.PendingMigrations = migrator.GetPendingMigrations();
            ViewBag.ErrorMessage = errorMsg;

            return View();
        }

        [Route("migration/run")]
        public Microsoft.AspNetCore.Mvc.ActionResult RunMigrations()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);

            try
            {
                migrator.Update();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Migrations", new { errorMsg = ex.Message });
            }

            return RedirectToAction("Migrations");
        }

    }
}