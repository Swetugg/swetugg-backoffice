﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class SpeakerAdminController : ConferenceAdminControllerBase
    {
        private readonly IImageUploader _imageUploader;
        private readonly string _speakerImageContainerName;

        public SpeakerAdminController(IImageUploader imageUploader, ApplicationDbContext dbContext) : base(dbContext)
        {
            _imageUploader = imageUploader;
            _speakerImageContainerName = ConfigurationManager.AppSettings["Storage.Container.Speakers.SpeakerImages"];
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
        public async Task<ActionResult> Speaker(int id, string errorMsg)
        {
            var conferenceId = ConferenceId;
            var speaker = await dbContext.Speakers
                .Include(sp => sp.Sessions.Select(s => s.Session))
                .Include(s => s.Tags)
                .Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == id);

            var imageTypes = await dbContext.ImageTypes.Where(it => it.ConferenceId == conferenceId).ToListAsync();
            var tags = await dbContext.Tags.Where(m => m.ConferenceId == conferenceId && m.Type == TagType.Speaker)
                .OrderBy(s => s.Name).ToListAsync();

            ViewBag.ImageTypes = imageTypes;
            ViewBag.ErrorMessage = errorMsg;
            ViewBag.Tags = tags;

            return View(speaker);
        }

        [Route("{conferenceSlug}/speakers/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var speaker = await dbContext.Speakers.SingleAsync(s => s.Id == id);
            return View(speaker);
        }

        [ValidateAntiForgeryToken]
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

                    return RedirectToAction("Speaker", new {speaker.Id});
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

        [ValidateAntiForgeryToken]
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
                    return RedirectToAction("Speaker", new {speaker.Id});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }

            return View(speaker);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/speakers/delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var speaker = await dbContext.Speakers.Include(sp => sp.Sessions)
                .SingleAsync(s => s.Id == id);

            foreach (var session in speaker.Sessions.ToArray())
            {
                dbContext.Entry(session).State = EntityState.Deleted;
            }

            dbContext.Entry(speaker).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/speakers/addtag/{id:int}")]
        public async Task<ActionResult> AddTag(int id, int tagId)
        {
            var speaker = await dbContext.Speakers.Include(m => m.Tags).SingleAsync(s => s.Id == id);
            var tag = await dbContext.Tags.FirstAsync(t => t.Id == tagId);
            speaker.Tags.Add(tag);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Speaker", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/speakers/removetag/{id:int}")]
        public async Task<ActionResult> RemoveTag(int id, int tagId)
        {
            var speaker = await dbContext.Speakers.Include(m => m.Tags).Where(s => s.Id == id).SingleAsync();

            foreach (var tag in speaker.Tags)
            {
                if (tag.Id == tagId)
                {
                    speaker.Tags.Remove(tag);
                    break;
                }
            }

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Speaker", new { id });
        }

        [Route("{conferenceSlug}/speakers/{speakerId:int}/image/{id:int}")]
        public async Task<ActionResult> Image(int speakerId, int id)
        {
            var conferenceId = ConferenceId;
            var speaker = await dbContext.Speakers.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == speakerId);
            var imageTypes = await dbContext.ImageTypes.Where(it => it.ConferenceId == conferenceId).ToListAsync();

            var image = speaker.Images.FirstOrDefault(i => i.Id == id);
            ViewBag.Speaker = speaker;
            ViewBag.ImageTypes = imageTypes;

            return View(image);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/speakers/{speakerId:int}/image/new")]
        public async Task<ActionResult> NewImage(int speakerId, SpeakerImage speakerImage, HttpPostedFileBase imageFile)
        {
            var speaker = await dbContext.Speakers.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == speakerId);
            var imageType = await dbContext.ImageTypes.FirstOrDefaultAsync(it => it.Id == speakerImage.ImageTypeId);

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
                            imageUrl = _imageUploader.UploadToStorage(memStream, speaker.Slug + "-" + imageType.Slug,
                                _speakerImageContainerName);
                        }

                        speakerImage.ImageUrl = imageUrl;

                        speaker.Images.Add(speakerImage);
                        await dbContext.SaveChangesAsync();
                        return RedirectToAction("Speaker", new {id = speakerId});
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

            return RedirectToAction("Speaker", new {id = speakerId, errorMsg = errorMessage});
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/speakers/{speakerId:int}/image/{id:int}/delete")]
        public async Task<ActionResult> DeleteImage(int id, int speakerId)
        {
            var speaker = await dbContext.Speakers.Include(sp => sp.Images.Select(i => i.ImageType))
                .SingleAsync(s => s.Id == speakerId);
            var image = speaker.Images.Single(i => i.Id == id);

            dbContext.Entry(image).State = EntityState.Deleted;
            _imageUploader.DeleteImage(image.ImageUrl);

            await dbContext.SaveChangesAsync();
            return RedirectToAction("Speaker", new {id = speakerId});
        }


        //TODO: fix integration from Sessionize
        private async Task<List<SpeakerImage>> SaveImageFromSessionize(string url, Speaker speaker)
        {
            //speaker.Images = await SaveImage(sessionizeSpeaker.profilePicture, speaker);
            //await dbContext.SaveChangesAsync();

            var imageTypes = await dbContext.ImageTypes.ToListAsync();
            var result = new List<SpeakerImage>();
            foreach (var imageType in imageTypes)
            {
                var speakerImage = new SpeakerImage()
                {
                    ImageType = imageType,
                    SpeakerId = speaker.Id,
                    ImageTypeId = imageType.Id
                };
                var request = WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                using (var response = await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        speakerImage.ImageUrl = _imageUploader.UploadToStorage(responseStream,
                            speaker.Slug + "-" + imageType.Slug, _speakerImageContainerName);
                    }
                }

                result.Add(speakerImage);



            }

            return result;

        }
    }
}