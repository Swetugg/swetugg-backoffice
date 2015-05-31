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
        private readonly IImageUploader _imageUploader;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _cfpSpeakerImageContainerName;

        public CallForPapersController(IConferenceService conferenceService, IImageUploader imageUploader, ApplicationDbContext dbContext)
        {
            _conferenceService = conferenceService;
            _imageUploader = imageUploader;
            _dbContext = dbContext;
            _cfpSpeakerImageContainerName = ConfigurationManager.AppSettings["Storage.Container.CallForPaper.SpeakerImages"];
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
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var conferences = _conferenceService.GetConferences();
            var cfpAlreadyCreatedFor = _dbContext.CfpSpeakers.Where(sp => sp.UserId == userId).Select(sp=>sp.ConferenceId).ToArray();

            ViewBag.CfpAlreadyCreatedFor = cfpAlreadyCreatedFor;

            return View(conferences);
        }

        [Route("{conferenceSlug}")]
        public ActionResult Conference(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                if (conference.IsCfpOpen())
                {
                    return RedirectToAction("Speaker");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Conference = conference;

            return View(speaker);
        }

        [Route("{conferenceSlug}/speaker")]
        public ActionResult Speaker(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                if (!conference.IsCfpOpen())
                {
                    // CFP Isn't open. Can't create new speakers.
                    return RedirectToAction("Index");
                }
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
        public ActionResult Speaker(string conferenceSlug, CfpSpeaker speaker, HttpPostedFileBase imageFile)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {
                try
                {
                    var dbSpeaker = 
                        _dbContext.CfpSpeakers.SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

                    if (dbSpeaker == null)
                    {
                        if (!conference.IsCfpOpen())
                        {
                            // CFP Isn't open. Can't create new speakers.
                            return RedirectToAction("Index");
                        }
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
                                    imageFile.InputStream.CopyTo(memStream);
                                    var imageUrl = _imageUploader.UploadToStorage(memStream, dbSpeaker.Email, _cfpSpeakerImageContainerName);
                                    dbSpeaker.Image = imageUrl;
                                }
                            }
                            catch (ImageUploadException e)
                            {
                                ModelState.AddModelError("ImageFile", e.Message);
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
                        _dbContext.SaveChanges();

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
        public ActionResult Session(string conferenceSlug, int? id)
        #pragma warning restore 0108
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            CfpSession session;
            if (id.HasValue)
            {
                session = speaker.Sessions.SingleOrDefault(s => s.Id == id);
            }
            else
            {
                if (!conference.IsCfpOpen())
                {
                    // CFP Isn't open. Can't create new sessions.
                    return RedirectToAction("Conference");
                }
                session = new CfpSession() { Speaker = speaker };
            }

            ViewBag.Conference = conference;
            ViewBag.Speaker = speaker;

            return View(session);
        }
        

        [Route("{conferenceSlug}/session")]
        [HttpPost]
        #pragma warning disable 0108
        public ActionResult Session(string conferenceSlug, int? id, CfpSession session)
        #pragma warning restore 0108
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            session.Speaker = speaker;

            if (ModelState.IsValid)
            {
                try
                {
                    var dbSession = id.HasValue ? speaker.Sessions.SingleOrDefault(s => s.Id == id) : null;
                    if (dbSession == null)
                    {
                        if (!conference.IsCfpOpen())
                        {
                            // CFP Isn't open. Can't create new sessions.
                            return RedirectToAction("Conference");
                        }
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

                    _dbContext.SaveChanges();
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
        public ActionResult DeleteSession(string conferenceSlug, int id)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            var session = speaker.Sessions.SingleOrDefault(s => s.Id == id);
            if (session != null)
            {
                _dbContext.Entry(session).State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Conference", new { conferenceSlug });
        }

        [HttpPost]
        [Route("{conferenceSlug}/delete-speaker")]
        public ActionResult DeleteSpeaker(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = User.Identity.GetUserId();
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            if (speaker != null)
            {
                foreach (var session in speaker.Sessions.ToArray())
                {
                    _dbContext.Entry(session).State = EntityState.Deleted;
                }
                _dbContext.Entry(speaker).State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}