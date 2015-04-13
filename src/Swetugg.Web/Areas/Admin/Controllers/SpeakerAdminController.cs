using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class SpeakerAdminController : ConferenceAdminControllerBase
    {

        public SpeakerAdminController(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

		[Route("{conferenceSlug}/speakers")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var speakers = from s in dbContext.Speakers
                           where s.ConferenceId == conferenceId
                           orderby s.Name
                           select s;

            var speakersList = await speakers.ToListAsync();
            return View(speakersList);
        }

		[Route("{conferenceSlug}/speakers/{id:int}")]
        public async Task<ActionResult> Speaker(int id)
        {
            var speaker = await dbContext.Speakers.Include(sp => sp.Sessions.Select(s => s.Session)).SingleAsync(s => s.Id == id);
            return View(speaker);
        }

		[Route("{conferenceSlug}/speakers/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var speaker = await dbContext.Speakers.SingleAsync(s => s.Id == id);
            return View(speaker);
        }

        [HttpPost]
		[Route("{conferenceSlug}/speakers/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    speaker.ConferenceId = ConferenceId;
					dbContext.Entry(speaker).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Speaker", new { speaker.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(speaker);
        }

		[Route("{conferenceSlug}/speakers/new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
		[Route("{conferenceSlug}/speakers/new", Order = 2)]
        public async Task<ActionResult> Edit(Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    speaker.ConferenceId = ConferenceId;
                    dbContext.Speakers.Add(speaker);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Speaker", new { speaker.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(speaker);
        }

        [HttpPost]
        [Route("{conferenceSlug}/speakers/delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var speaker = await dbContext.Speakers.Include(sp => sp.Sessions).Include(sp => sp.CfpSpeakers).SingleAsync(s => s.Id == id);

            foreach (var session in speaker.Sessions.ToArray())
            {
                dbContext.Entry(session).State = EntityState.Deleted;
            }
            speaker.CfpSpeakers.Clear();

            dbContext.Entry(speaker).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}