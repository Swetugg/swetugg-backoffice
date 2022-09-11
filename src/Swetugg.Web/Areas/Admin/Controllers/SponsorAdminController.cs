using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class SponsorAdminController : ConferenceAdminControllerBase
    {
        private readonly IImageUploader _imageUploader;
        private readonly string _sponsorImageContainerName;

        public SponsorAdminController(IImageUploader imageUploader, ApplicationDbContext dbContext) : base(dbContext)
        {
            _imageUploader = imageUploader;
            _sponsorImageContainerName = ConfigurationManager.AppSettings["Storage.Container.Sponsors.SponsorImages"];
        }

        [Route("{conferenceSlug}/sponsors")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
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
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Sponsor(int id)
        {
            var conferenceId = ConferenceId;
            var sponsor = await dbContext.Sponsors
                .Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == id);
            
            var imageTypes = await dbContext.ImageTypes.Where(it => it.ConferenceId == conferenceId).ToListAsync();
            ViewBag.ImageTypes = imageTypes;

            return View(sponsor);
        }

		[Route("{conferenceSlug}/sponsors/edit/{id:int}", Order = 1)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id)
        {
            var sponsor = await dbContext.Sponsors.SingleAsync(s => s.Id == id);
            return View(sponsor);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/sponsors/edit/{id:int}", Order = 1)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id, Sponsor sponsor)
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
        public Microsoft.AspNetCore.Mvc.ActionResult Edit()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/sponsors/new", Order = 2)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(Sponsor sponsor)
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

        [Route("{conferenceSlug}/sponsors/{sponsorId:int}/image/{id:int}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Image(int sponsorId, int id)
        {
            var conferenceId = ConferenceId;
            var sponsor = await dbContext.Sponsors.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == sponsorId);
            var imageTypes = await dbContext.ImageTypes.Where(it => it.ConferenceId == conferenceId).ToListAsync();

            var image = sponsor.Images.FirstOrDefault(i => i.Id == id);
            ViewBag.Sponsor = sponsor;
            ViewBag.ImageTypes = imageTypes;

            return View(image);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sponsors/{sponsorId:int}/image/new")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> NewImage(int sponsorId, SponsorImage sponsorImage, HttpPostedFileBase imageFile)
        {
            var sponsor = await dbContext.Sponsors.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == sponsorId);
            var imageType = await dbContext.ImageTypes.FirstOrDefaultAsync(it => it.Id == sponsorImage.ImageTypeId);

            if (imageType == null)
            {
                throw new InvalidOperationException("Unknown image type");
            }

            string errorMessage = null;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (imageFile.ContentLength < 10 * 1024 * 1024)
                {
                    try
                    {
                        string imageUrl;
                        using (var memStream = new MemoryStream())
                        {
                            imageFile.InputStream.CopyTo(memStream);
                            imageUrl = _imageUploader.UploadToStorage(memStream, sponsor.Slug + "-" + imageType.Slug,
                                _sponsorImageContainerName);
                        }

                        sponsorImage.ImageUrl = imageUrl;

                        sponsor.Images.Add(sponsorImage);
                        await dbContext.SaveChangesAsync();
                        return RedirectToAction("Sponsor", new { id = sponsorId });
                    }
                    catch (ImageUploadException e)
                    {
                        errorMessage = e.Message;
                        ModelState.AddModelError("ImageFile", e.Message);
                    }
                }
                else
                {
                    errorMessage = "Max file size of image is ~10MB";
                    ModelState.AddModelError("ImageFile", "Max file size of image is ~10MB");
                }
            }

            return RedirectToAction("Sponsor", new { id = sponsorId, errorMsg = errorMessage });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sponsors/{sponsorId:int}/image/{id:int}/delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DeleteImage(int id, int sponsorId)
        {
            var sponsor = await dbContext.Sponsors.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == sponsorId);
            var image = sponsor.Images.Single(i => i.Id == id);

            dbContext.Entry(image).State = EntityState.Deleted;
            _imageUploader.DeleteImage(image.ImageUrl);

            await dbContext.SaveChangesAsync();
            return RedirectToAction("Sponsor", new { id = sponsorId });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sponsors/delete/{id:int}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Delete(int id)
        {
            var sponsor = await dbContext.Sponsors.SingleAsync(sp => sp.Id == id);

            dbContext.Entry(sponsor).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();
            
            return RedirectToAction("Index");

        }
    }
}