using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class TagAdminController : ConferenceAdminControllerBase
    {
        public TagAdminController(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        [Route("{conferenceSlug}/tags")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var tags = from s in dbContext.Tags
                where s.ConferenceId == conferenceId
                orderby s.Priority
                select s;
            var tagList = await tags.ToListAsync();

            return View(tagList);
        }

        [Route("{conferenceSlug}/tags/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var tag = await dbContext.Tags.SingleAsync(s => s.Id == id);
            return View(tag);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/tags/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Tag tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tag.ConferenceId = ConferenceId;
                    dbContext.Entry(tag).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(tag);
        }

        [Route("{conferenceSlug}/tags/new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/tags/new", Order = 2)]
        public async Task<ActionResult> Edit(Tag tag)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tag.ConferenceId = ConferenceId;
                    dbContext.Tags.Add(tag);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(tag);
        }

    }
}