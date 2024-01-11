using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Swetugg.Web.Areas.Admin.Controllers.Models;
using Swetugg.Web.Models;
using Swetugg.Web.Controllers;
using Swetugg.Web.Services;
using System.Configuration;
using Microsoft.Ajax.Utilities;

namespace Swetugg.Web.Areas.Admin.Controllers
{

    public class SessionizeController : ConferenceAdminControllerBase
    {
        private readonly IImageUploader _imageUploader;
        private readonly string _speakerImageContainerName;
        public static string baseurl;

        public SessionizeController(IImageUploader imageUploader, Web.Models.ApplicationDbContext dbContext) : base(dbContext)
        {
            _imageUploader = imageUploader;
            _speakerImageContainerName = ConfigurationManager.AppSettings["Storage.Container.Speakers.SpeakerImages"];
        }


        [Route("{conferenceSlug}/Sessionize")]
        public async Task<ActionResult> Index()
        {
            baseurl = $"api/v2/{this.Conference.SessionizeAPICode}/view";
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

        public static async Task<List<SezzionizeSchedule>> GetScheduleFromSessionize()
        {
            List<SezzionizeSchedule> schedule;
            try
            {

                var request = WebRequest.Create($"https://sessionize.com/" + baseurl + "/GridSmart");
                request.Method = WebRequestMethods.Http.Get;
                request.ContentType = "application/json; charset=utf-8";
                using (var response = await request.GetResponseAsync())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            var text = streamReader.ReadToEnd();
                            schedule = JsonConvert.DeserializeObject<List<SezzionizeSchedule>>(text);
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                throw;
            }
            return schedule;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/Sessionize/ImportSpeaker")]
        public async Task<ActionResult> ImportSpeaker()
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

                foreach (var link in sessionizeSpeaker.links)
                {
                    switch (link.title)
                    {
                        case "Twitter":
                            speaker.Twitter = link.url;
                            break;
                        case "Company Website":
                            speaker.Web = link.url;
                            break;
                        case "Blog":
                            speaker.Blog = link.url;
                            break;
                        case "LinkedIn":
                            speaker.LinkedIn = link.url;
                            break;
                        default:
                            break;
                    }
                }

                dbContext.Entry(speaker).State = EntityState.Added;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/Sessionize/ImportSchedule")]
        public async Task<ActionResult> ImportSchedule()
        {
            try
            {
                var sessionizeSchedule = await GetScheduleFromSessionize();

                // rooms

                //var ExistingRooms = dbContext.Rooms.Where(r => r.ConferenceId == ConferenceId).ToList();

                //var rooms = sessionizeSchedule.SelectMany(s => s.Rooms).DistinctBy(r => r.Id).Select(r => new
                //{
                //    SessionizeId = r.Id,
                //    ConferenceId = ConferenceId,
                //    RoomId = ExistingRooms.Where(er => er.SessionizeId == r.Id).Select(er => er.Id).FirstOrDefault(),
                //    r.Name,
                //    Slug = r.Name.Slugify(),
                //    Priority = 100
                //}).ToList();

                //var roomsnotindb = rooms.Where(r => r.RoomId == 0).ToList();



                //foreach (var room in roomsnotindb)
                //{
                //    var roomEntity = new Room()
                //    {
                //        ConferenceId = ConferenceId,
                //        SessionizeId = room.SessionizeId,
                //        Name = room.Name,
                //        Slug = room.Slug,
                //        Priority = room.Priority
                //    };
                //    dbContext.Entry(roomEntity).State = EntityState.Added;
                //}

                // slots

                //var slots = sessionizeSchedule.SelectMany(s => s.Rooms.SelectMany(r => r.Sessions.Select(se => new { se.StartsAt, se.EndsAt, se.IsPlenumSession, Title = se.IsPlenumSession ? se.Title : "" }))).Distinct().Select(se => new { start = DateTime.Parse(se.StartsAt), end = DateTime.Parse(se.EndsAt), se.IsPlenumSession, Title = se.Title, id = Guid.NewGuid() }).ToList();


                //var plenumSlots = slots.Where(s => s.IsPlenumSession).ToList();

                ////select the slot that have a start time inside a plenumslot start and end
                //var lunchslots = slots.Where(s => s.IsPlenumSession == false).Where(s => plenumSlots.Any(ps => s.start >= ps.start && s.end <= ps.end));

                //var realslots = slots.Where(s => !lunchslots.Any(ls => ls.id == s.id));

                //var ExistingSlots = dbContext.Slots.Where(r => r.ConferenceId == ConferenceId).ToList();

                //var newSlots = realslots.Where(rs => !ExistingSlots.Any(ex => rs.start == ex.Start && rs.end == ex.End)).ToList();

                //foreach (var slot in newSlots)
                //{
                //    var slotEntity = new Slot()
                //    {
                //        ConferenceId = ConferenceId,
                //        Title = slot.Title,
                //        Start = slot.start,
                //        End = slot.end,
                //        HasSessions = slot.IsPlenumSession,
                //    };
                //    dbContext.Entry(slotEntity).State = EntityState.Added;
                //}

                //roomslots
                var sessions = sessionizeSchedule.SelectMany(s => s.Rooms.SelectMany(r => r.Sessions.Select(se => new { SessionSessionizeId = se.Id, RoomSessionizeId = se.RoomId, Start = DateTime.Parse(se.StartsAt), End = DateTime.Parse(se.EndsAt) })));

                //hämta befintliga roomslots

                //skapa entiterna, special om de faller inom tiden för en lunchslot samt special om de är lunchslot

                //jämför dessa

                var i = 0;
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }



            return RedirectToAction("Index");
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/Sessionize/ImportSession")]
        public async Task<ActionResult> ImportSession()
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