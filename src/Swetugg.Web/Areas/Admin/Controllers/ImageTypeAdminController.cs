using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class ImageTypeAdminController : ConferenceAdminControllerBase
    {
        public ImageTypeAdminController(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        [Route("{conferenceSlug}/image-types")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var imageTypes = from s in dbContext.ImageTypes
                where s.ConferenceId == conferenceId
                select s;
            var imageTypeList = await imageTypes.ToListAsync();

            return View(imageTypeList);
        }

        [Route("{conferenceSlug}/image-types/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var imageType = await dbContext.ImageTypes.SingleAsync(s => s.Id == id);
            return View(imageType);
        }

        [HttpPost]
        [Route("{conferenceSlug}/image-types/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, ImageType imageType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    imageType.ConferenceId = ConferenceId;
                    dbContext.Entry(imageType).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(imageType);
        }

        [Route("{conferenceSlug}/image-types/new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [Route("{conferenceSlug}/image-types/new", Order = 2)]
        public async Task<ActionResult> Edit(ImageType imageType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    imageType.ConferenceId = ConferenceId;
                    dbContext.ImageTypes.Add(imageType);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(imageType);
        }

    }
}