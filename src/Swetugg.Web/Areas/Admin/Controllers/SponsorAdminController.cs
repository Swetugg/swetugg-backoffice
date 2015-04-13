using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class SponsorAdminController : ConferenceAdminControllerBase
    {

        public SponsorAdminController(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

		[Route("{conferenceSlug}/sponsors")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var sponsors = from s in dbContext.Sponsors
                           where s.ConferenceId == conferenceId
                           orderby s.Name
                           select s;

            var sponsorsList = await sponsors.ToListAsync();
            return View(sponsorsList);
        }

		[Route("{conferenceSlug}/sponsors/{id:int}")]
        public async Task<ActionResult> Sponsor(int id)
        {
            var sponsor = await dbContext.Sponsors.SingleAsync(s => s.Id == id);
            return View(sponsor);
        }

		[Route("{conferenceSlug}/sponsors/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var sponsor = await dbContext.Sponsors.SingleAsync(s => s.Id == id);
            return View(sponsor);
        }

        [HttpPost]
		[Route("{conferenceSlug}/sponsors/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    sponsor.ConferenceId = ConferenceId;
					dbContext.Entry(sponsor).State = EntityState.Modified;
					await dbContext.SaveChangesAsync();

                    return RedirectToAction("Sponsor", new { sponsor.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(sponsor);
        }

		[Route("{conferenceSlug}/sponsors/new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
		[Route("{conferenceSlug}/sponsors/new", Order = 2)]
        public async Task<ActionResult> Edit(Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    sponsor.ConferenceId = ConferenceId;
                    dbContext.Sponsors.Add(sponsor);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Sponsor", new { sponsor.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(sponsor);
        }

        [HttpPost]
        [Route("{conferenceSlug}/sponsors/delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var sponsor = await dbContext.Sponsors.SingleAsync(sp => sp.Id == id);

            dbContext.Entry(sponsor).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();
            
            return RedirectToAction("Index");

        }
    }
}