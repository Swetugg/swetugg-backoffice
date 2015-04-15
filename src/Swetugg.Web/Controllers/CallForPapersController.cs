using System;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Instrumentation;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Controllers
{
    [RequireHttps]
    [RoutePrefix("cfp")]
    [Authorize()]
    public class CallForPapersController : Controller
    {
        private readonly IConferenceService _conferenceService;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _dbContext;

        public CallForPapersController(IConferenceService conferenceService, ApplicationDbContext dbContext)
        {
            _conferenceService = conferenceService;
            _dbContext = dbContext;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [Route("")]
        public async Task<ActionResult> Index()
        {
            var conferences = await _conferenceService.GetConferences();
            return View(conferences);
        }

        [Route("{conferenceSlug}")]
        public async Task<ActionResult> Conference(string conferenceSlug)
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                return RedirectToAction("Speaker");
            }
            ViewBag.Conference = conference;

            return View(speaker);
        }

        [Route("{conferenceSlug}/speaker")]
        public async Task<ActionResult> Speaker(string conferenceSlug)
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                speaker = new CfpSpeaker()
                {
                    Email = User.Identity.Name
                };
            }
            ViewBag.Conference = conference;

            return View(speaker);
        }

        [Route("{conferenceSlug}/speaker")]
        [HttpPost]
        public async Task<ActionResult> Speaker(string conferenceSlug, CfpSpeaker speaker, HttpPostedFileBase imageFile)
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {
                try
                {
                    var dbSpeaker = await
                        _dbContext.CfpSpeakers.SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

                    if (dbSpeaker == null)
                    {
                        dbSpeaker = new CfpSpeaker();
                        _dbContext.Entry(dbSpeaker).State = EntityState.Added;
                        dbSpeaker.UserId = userId;
                    }
                    dbSpeaker.ConferenceId = conference.Id;
                    dbSpeaker.Bio = speaker.Bio;
                    dbSpeaker.Company = speaker.Company;
                    dbSpeaker.Email = speaker.Email;
                    dbSpeaker.GitHub = speaker.GitHub;
                    dbSpeaker.Name = speaker.Name;
                    dbSpeaker.Twitter = speaker.Twitter;
                    dbSpeaker.Web = speaker.Web;

                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        if (imageFile.ContentLength < 10*1024*1024)
                        {
                            try
                            {
                                using (var memStream = new MemoryStream())
                                {
                                    await imageFile.InputStream.CopyToAsync(memStream);

                                    var parsedImage = Image.FromStream(memStream);

                                    try
                                    {
                                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                                            ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);

                                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                                        CloudBlobContainer container =
                                            blobClient.GetContainerReference("cfp-speaker-images");
                                        container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

                                        var speakerFileName = "speaker-image-" + dbSpeaker.Email + "-" + Guid.NewGuid();

                                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(speakerFileName);
                                        blockBlob.Properties.ContentType = parsedImage.RawFormat.GetMimeType();
                                        memStream.Seek(0, SeekOrigin.Begin);
                                        blockBlob.UploadFromStream(memStream);

                                        dbSpeaker.Image = blockBlob.Uri.ToString();
                                    }
                                    catch (Exception)
                                    {
                                        ModelState.AddModelError("ImageFile", "File upload failed");
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("ImageFile",
                                    "Unable to parse file as image. Please use JPG or PNG files");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ImageFile", "Max file size of image is ~10MB");
                        }
                    }
                    // If there are still no errors
                    if (ModelState.IsValid)
                    {
                        await _dbContext.SaveChangesAsync();

                        return RedirectToAction("Conference");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Saving", ex);
                }
            }

            ViewBag.Conference = conference;
            return View(speaker);
        }

        [Route("{conferenceSlug}/session")]
        #pragma warning disable 0108
        public async Task<ActionResult> Session(string conferenceSlug, int? id)
        #pragma warning restore 0108
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            var session = id.HasValue ? speaker.Sessions.SingleOrDefault(s => s.Id == id) : new CfpSession() { Speaker = speaker };

            ViewBag.Conference = conference;
            ViewBag.Speaker = speaker;

            return View(session);
        }
        

        [Route("{conferenceSlug}/session")]
        [HttpPost]
        #pragma warning disable 0108
        public async Task<ActionResult> Session(string conferenceSlug, int? id, CfpSession session)
        #pragma warning restore 0108
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            session.Speaker = speaker;

            if (ModelState.IsValid)
            {
                try
                {
                    var dbSession = id.HasValue ? speaker.Sessions.SingleOrDefault(s => s.Id == id) : null;
                    if (dbSession == null)
                    {
                        dbSession = new CfpSession();
                        _dbContext.Entry(dbSession).State = EntityState.Added;
                        dbSession.ConferenceId = conference.Id;
                        speaker.Sessions.Add(dbSession);
                    }
                    dbSession.Name = session.Name;
                    dbSession.Description = session.Description;
                    dbSession.Comments = session.Comments;
                    dbSession.Audience = session.Audience;
                    dbSession.Level = session.Level;
                    dbSession.Tags = session.Tags;

                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Conference");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Saving", ex);
                }
            }

            ViewBag.Conference = conference;
            ViewBag.Speaker = speaker;
            return View(session);
        }

        [HttpPost]
        [Route("{conferenceSlug}/delete-session")]
        public async Task<ActionResult> DeleteSession(string conferenceSlug, int id)
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            var session = speaker.Sessions.SingleOrDefault(s => s.Id == id);
            if (session != null)
            {
                _dbContext.Entry(session).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Conference", new { conferenceSlug });
        }

        [HttpPost]
        [Route("{conferenceSlug}/delete-speaker")]
        public async Task<ActionResult> DeleteSpeaker(string conferenceSlug)
        {
            var conference = await _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = await _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefaultAsync(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            if (speaker != null)
            {
                foreach (var session in speaker.Sessions.ToArray())
                {
                    _dbContext.Entry(session).State = EntityState.Deleted;
                }
                _dbContext.Entry(speaker).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}