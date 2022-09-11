using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Areas.Cfp.Controllers
{

    [RequireHttps]
    [Area("Cfp")]
    [Authorize()]
    public class CallForPapersController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConferenceService _conferenceService;
        private readonly IImageUploader _imageUploader;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ApplicationDbContext _dbContext;
        private readonly string _cfpSpeakerImageContainerName;

        public CallForPapersController(
            IConferenceService conferenceService,
            IImageUploader imageUploader,
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _conferenceService = conferenceService;
            _imageUploader = imageUploader;
            _dbContext = dbContext;
            _cfpSpeakerImageContainerName = ConfigurationManager.AppSettings["Storage.Container.CallForPaper.SpeakerImages"];
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("")]
        //[OverrideAuthorization]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var conferences = _conferenceService.GetConferences().Where(c => c.End == null || c.End >= ConferenceInfoExtensions.CurrentTime(c).Date);
            var cfpAlreadyCreatedFor = _dbContext.CfpSpeakers.Where(sp => sp.UserId == userId).Select(sp=>sp.ConferenceId).ToArray();

            ViewBag.CfpAlreadyCreatedFor = cfpAlreadyCreatedFor;

            return View(conferences);
        }


        [Route("{conferenceSlug}/vip-invite/{code}")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Invite(string conferenceSlug, string code)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);

            if (conference.CfpVipCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
            {
                var user = await _userManager.GetUserAsync(User);
                
                await _userManager.AddToRoleAsync(user, "VipSpeaker");
                await _signInManager.SignInAsync(user, false); // Sign in again
                ViewBag.Success = true;
            }
            else
            {
                ViewBag.Success = false;
            }

            ViewBag.Conference = conference;
            return View(conference);
        }

        [Route("{conferenceSlug}")]
        public Microsoft.AspNetCore.Mvc.ActionResult Conference(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                if (User.IsAllowedToCreateCfp(conference))
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
        public Microsoft.AspNetCore.Mvc.ActionResult Speaker(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            if (speaker == null)
            {
                if (!User.IsAllowedToCreateCfp(conference))
                {
                    // CFP Isn't open. Can't create new speakers.
                    return RedirectToAction("Index");
                }
                speaker = new CfpSpeaker()
                {
                    Email = User.Identity.Name,
                    NeedAccommodation = true,
                    NeedTravel = true
                };
            }
            ViewBag.Conference = conference;

            return View(speaker);
        }

        [ValidateAntiForgeryToken]
        [Route("{conferenceSlug}/speaker")]
        [HttpPost]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Speaker(string conferenceSlug, CfpSpeaker speaker, IFormFile imageFile)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                try
                {
                    var dbSpeaker = 
                        _dbContext.CfpSpeakers.SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

                    if (dbSpeaker == null)
                    {
                        if (!User.IsAllowedToCreateCfp(conference))
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
                    dbSpeaker.Phone = speaker.Phone;
                    dbSpeaker.CountryOfResidence = speaker.CountryOfResidence;
                    dbSpeaker.Comments = speaker.Comments;
                    dbSpeaker.NeedTravel = speaker.NeedTravel;
                    dbSpeaker.NeedAccommodation = speaker.NeedAccommodation;
                    dbSpeaker.LastUpdate = DateTime.UtcNow;

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (imageFile.Length < 10*1024*1024)
                        {
                            try
                            {
                                using (var memStream = new MemoryStream())
                                {
                                    imageFile.OpenReadStream().CopyTo(memStream);
                                    var imageUrl = await _imageUploader.UploadToStorage(memStream, dbSpeaker.Email, _cfpSpeakerImageContainerName);
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
                    // TODO: Add model error
                    //ModelState.AddModelError("Saving", ex);
                }
            }

            ViewBag.Conference = conference;
            return View(speaker);
        }

        [Route("{conferenceSlug}/session")]
        #pragma warning disable 0108
        public Microsoft.AspNetCore.Mvc.ActionResult Session(string conferenceSlug, int? id)
        #pragma warning restore 0108
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            var speaker = _dbContext.CfpSpeakers
                .Include(sp => sp.Sessions.Select(se => se.SessionType))
                .SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            CfpSession session;
            if (id.HasValue)
            {
                session = speaker.Sessions.SingleOrDefault(s => s.Id == id);
            }
            else
            {
                if (!User.IsAllowedToCreateCfp(conference))
                {
                    // CFP Isn't open. Can't create new sessions.
                    return RedirectToAction("Conference");
                }
                session = new CfpSession() { Speaker = speaker };
            }

            ViewBag.Conference = conference;
            ViewBag.Speaker = speaker;
            ViewBag.SessionTypes = _dbContext.SessionTypes.Where(m => m.ConferenceId == conference.Id).OrderBy(s => s.Priority).ToList();

            return View(session);
        }

        [ValidateAntiForgeryToken]
        [Route("{conferenceSlug}/session")]
        [HttpPost]
        #pragma warning disable 0108
        public Microsoft.AspNetCore.Mvc.ActionResult Session(string conferenceSlug, int? id, CfpSession session)
        #pragma warning restore 0108
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            var speaker = _dbContext.CfpSpeakers
                .Include(sp => sp.Sessions.Select(se => se.SessionType))
                .SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);
            session.Speaker = speaker;

            if (ModelState.IsValid)
            {
                try
                {
                    var dbSession = id.HasValue ? speaker.Sessions
                        .SingleOrDefault(s => s.Id == id) : null;
                    if (dbSession == null)
                    {
                        if (!User.IsAllowedToCreateCfp(conference))
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
                    dbSession.LastUpdate = DateTime.UtcNow;
                    dbSession.SessionTypeId = session.SessionTypeId;

                    _dbContext.SaveChanges();
                    return RedirectToAction("Conference");
                }
                catch (Exception ex)
                {
                    // TODO: Add model error
                    //ModelState.AddModelError("Saving", ex);
                }
            }

            ViewBag.SessionTypes = _dbContext.SessionTypes.Where(m => m.ConferenceId == conference.Id).OrderBy(s => s.Priority).ToList();
            ViewBag.Conference = conference;
            ViewBag.Speaker = speaker;
            return View(session);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/delete-session")]
        public Microsoft.AspNetCore.Mvc.ActionResult DeleteSession(string conferenceSlug, int id)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
            var speaker = _dbContext.CfpSpeakers.Include(sp => sp.Sessions).SingleOrDefault(sp => sp.UserId == userId && sp.ConferenceId == conference.Id);

            var session = speaker.Sessions.SingleOrDefault(s => s.Id == id);
            if (session != null)
            {
                _dbContext.Entry(session).State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Conference", new { conferenceSlug });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/delete-speaker")]
        public Microsoft.AspNetCore.Mvc.ActionResult DeleteSpeaker(string conferenceSlug)
        {
            var conference = _conferenceService.GetConferenceBySlug(conferenceSlug);
            var userId = _userManager.GetUserId(User);
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