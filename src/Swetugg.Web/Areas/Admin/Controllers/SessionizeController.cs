using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swetugg.Web.Areas.Admin.Controllers.Models;
using Swetugg.Web.Models;
using Swetugg.Web.Controllers;
using Swetugg.Web.Services;
using System.Configuration;

namespace Swetugg.Web.Areas.Admin.Controllers
{

    public class SessionizeController : ConferenceAdminControllerBase
    {
        private readonly IImageUploader _imageUploader;
        private readonly string _speakerImageContainerName;
        private static string baseurl = "api/v2/ckitpijn/view"; //TODO: move to conference (new field)

        public SessionizeController(IImageUploader imageUploader, Web.Models.ApplicationDbContext dbContext) : base(dbContext)
        {
            _imageUploader = imageUploader;
            _speakerImageContainerName = ConfigurationManager.AppSettings["Storage.Container.Speakers.SpeakerImages"];

        }


        [Route("{conferenceSlug}/Sessionize")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var m = new SessionizeSync();

            var Speakers = await GetSpeakerFromSessionize();

            var speakersAllreadyInDatabase = dbContext.Speakers
                .Where(s => s.SessionizeId.HasValue && s.ConferenceId == ConferenceId)
                .Select(s => s.SessionizeId.Value);

            m.Speakers = Speakers.Select(s => new SpeakerSync
            {
                SessionizeId = Guid.Parse(s.id),
                Name = s.fullName,
                AllreadyInDatabase = speakersAllreadyInDatabase.Contains(Guid.Parse(s.id))
            }).ToList();


            var sessionGroups = await GetSessionsFromSessionize();

            var sessionsAllreadyInDatabase = dbContext.Sessions
                .Where(s => s.SessionizeId.HasValue)
                .Select(s => s.SessionizeId.Value);

            m.Sessions = new List<SessionSync>();

            foreach (var sessionGroup in sessionGroups)
            {
                m.Sessions.AddRange(sessionGroup.sessions.Select(s => new SessionSync
                {
                    SessionizeId = int.Parse(s.id),
                    Title = s.title,
                    AllreadyInDatabase = sessionsAllreadyInDatabase.Contains(int.Parse(s.id))
                }).ToList());
            }


            return View(m);
        }

        private static async Task<List<SezzionizeSpeaker>> GetSpeakerFromSessionize()
        {
            List<SezzionizeSpeaker> Speakers;

            var request = WebRequest.Create($"https://sessionize.com/" + baseurl + "/Speakers");
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json; charset=utf-8";
            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var text = streamReader.ReadToEnd();
                        Speakers = JsonConvert.DeserializeObject<List<SezzionizeSpeaker>>(text);
                    }
                }
            }

            return Speakers;
        }

        private static async Task<List<SessionGroup>> GetSessionsFromSessionize()
        {
            List<SessionGroup> SessionGroups;

            var request = WebRequest.Create($"https://sessionize.com/" + baseurl + "/Sessions");
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/json; charset=utf-8";
            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream))
                    {
                        var text = streamReader.ReadToEnd();
                        SessionGroups = JsonConvert.DeserializeObject<List<SessionGroup>>(text);
                    }
                }
            }

            return SessionGroups;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/Sessionize/ImportSpeaker")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ImportSpeaker()
        {
            var sessionizeSpeakers = await GetSpeakerFromSessionize();

            foreach (var sessionizeSpeaker in sessionizeSpeakers)
            {
                var sessionizeId = Guid.Parse(sessionizeSpeaker.id);
                var speakerAlreadyExist = dbContext.Speakers.Any(s => s.SessionizeId == sessionizeId && s.ConferenceId == ConferenceId);
                if (speakerAlreadyExist)
                    continue;

                var speaker = new Speaker()
                {
                    ConferenceId = ConferenceId,
                    SessionizeId = sessionizeId,
                    FirstName = sessionizeSpeaker.firstName,
                    Name = sessionizeSpeaker.fullName,
                    Company = sessionizeSpeaker.tagLine,
                    Bio = sessionizeSpeaker.bio,
                    Slug = sessionizeSpeaker.fullName.Slugify(),
                    Priority = 100,
                    SessionizeImageUrl = sessionizeSpeaker.profilePicture,
                };

                var twitter = sessionizeSpeaker.links.FirstOrDefault(l => l.title == "Twitter");
                if (twitter != null)
                {
                    var charPosition = twitter.url.LastIndexOf("/");
                    speaker.Twitter = twitter.url.Remove(0, charPosition + 1);
                }

                var blog = sessionizeSpeaker.links.FirstOrDefault(l => l.title == "Blog");
                var podcast = sessionizeSpeaker.links.FirstOrDefault(l => l.title == "Podcast");
                var company = sessionizeSpeaker.links.FirstOrDefault(l => l.title == "Company Website");
                //Github
                if (blog != null)
                {
                    speaker.Web = blog.url;

                }
                else if (podcast != null)
                {
                    speaker.Web = podcast.url;
                }
                else if (company != null)
                {
                    speaker.Web = company.url;
                }


                foreach (var link in sessionizeSpeaker.links)
                {
                    switch (link.title)
                    {
                        case "Twitter":
                        case "Company Website":
                        case "Blog":
                        case "Podcast":
                            break;
                        default:
                            break;
                            //Github

                    }
                }

                dbContext.Entry(speaker).State = EntityState.Added;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/Sessionize/ImportSession")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ImportSession()
        {
            var sessionizeSessionGroups = await GetSessionsFromSessionize();

            foreach (var sessionizeSessions in sessionizeSessionGroups)
            {
                foreach (var sessionizeSession in sessionizeSessions.sessions)
                {
                    var sessionizeId = int.Parse(sessionizeSession.id);
                    var sessionAlreadyExist = dbContext.Sessions.Any(s => s.SessionizeId == sessionizeId);
                    if (sessionAlreadyExist)
                        continue;

                    var sessionType = dbContext.SessionTypes.First();

                    var session = new Session()
                    {
                        ConferenceId = ConferenceId,
                        SessionizeId = sessionizeId,
                        Name = sessionizeSession.title,
                        Description = sessionizeSession.description,
                        SessionType = sessionType,
                        Slug = sessionizeSession.title.Slugify(),
                        Priority = 100
                    };

                    dbContext.Entry(session).State = EntityState.Added;

                    await dbContext.SaveChangesAsync();

                    foreach (var sessionizeSpeaker in sessionizeSession.speakers)
                    {
                        var id = Guid.Parse(sessionizeSpeaker.id);
                        var speaker = await dbContext.Speakers.SingleOrDefaultAsync(s => s.SessionizeId.HasValue && s.ConferenceId == ConferenceId ? s.SessionizeId == id : false);
                        if (speaker != null)
                        {
                            var ss = new SessionSpeaker
                            {
                                SessionId = session.Id,
                                SpeakerId = speaker.Id
                            };

                            dbContext.Entry(ss).State = EntityState.Added;

                        }
                    }

                    await dbContext.SaveChangesAsync();

                }
            }

            return RedirectToAction("Index");

        }

    }
}