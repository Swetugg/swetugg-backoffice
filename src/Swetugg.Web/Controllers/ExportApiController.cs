using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Controllers
{
    namespace SpeakerCsv
    {
        public class Speaker
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Twitter { get; set; }
            public string Session { get; set; }
            public string Room { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }

    namespace Everens
    {
        public class Export
        {
            public Taxonomy[] taxonomy { get; set; }
            public Place[] places { get; set; }
            public Speaker[] speakers { get; set; }
            public Activity[] activities { get; set; }
            public Sponsor[] sponsors { get; set; }
        }

        public class Taxonomy
        {
            public string externalReferenceId { get; set; }
            public string title { get; set; }
            public Child[] children { get; set; }
        }

        public class Child
        {
            public string externalReferenceId { get; set; }
            public string title { get; set; }
        }

        public class Place
        {
            public string externalReferenceId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string address { get; set; }
        }

        public class Speaker
        {
            public string externalReferenceId { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string bio { get; set; }
            public string byline { get; set; }
            public string github { get; set; }
            public string twitter { get; set; }
            public string imageUrl { get; set; }
        }

        public class Activity
        {
            public string externalReferenceId { get; set; }
            public string[] externalTaxonomyReferenceIds { get; set; }
            public string startTime { get; set; }
            public string endTime { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string[] externalPlaceReferenceIds { get; set; }
            public string[] externalSpeakerReferenceIds { get; set; }
        }

        public class Sponsor
        {
            public string externalReferenceId { get; set; }
            public Information information { get; set; }
            public string externalTaxonomyReferenceId { get; set; }
        }

        public class Information
        {
            public string website { get; set; }
            public string title { get; set; }
        }

    }

    public class ExportApiController : Controller
    {
        private readonly IConferenceService conferenceService;

        private readonly ApplicationDbContext dbContext;
        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;

        public ExportApiController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.conferenceService = new CachedConferenceService(new ConferenceService(dbContext));
        }

        [Route("{conferenceSlug}/export/everens")]
        public async Task<ActionResult> Everens()
        {
            var conferenceId = ConferenceId;

            var speakers = conferenceService.GetSpeakers(conferenceId);
            
            var slots = conferenceService.GetSlotsAndSessions(conferenceId);
            var rooms = conferenceService.GetRooms(conferenceId);
            var sponsors = conferenceService.GetSponsors(conferenceId);
                
            var export = new Everens.Export
            {
                taxonomy = new [] {
                    new Everens.Taxonomy()
                    {
                        externalReferenceId = "breaks",
                        title = "Breaks"
                    },
                    new Everens.Taxonomy()
                    {
                        externalReferenceId = "sessions",
                        title = "Sessions",
                    }, 
                    new Everens.Taxonomy()
                    {
                        externalReferenceId = "language",
                        title = "Language",
                        children = new []
                        {
                            new Everens.Child() { externalReferenceId = "language_sv", title = "Swedish" }, 
                            new Everens.Child() { externalReferenceId = "language_en", title = "English" }, 
                        }
                    }, 
                    new Everens.Taxonomy()
                    {
                        externalReferenceId = "sponsors",
                        title = "Sponsors"
                    }, 
                }, 
                places = rooms.Select(r => new Everens.Place()
                {
                    title = r.Name,
                    description = r.Description,
                    externalReferenceId = "room_" + r.Id,
                }).ToArray(),
                speakers = speakers.Select(s =>
                {
                    var names = s.Name.Split(' ');
                    var firstName = names.First();
                    var lastName = "";
                    if (names.Length > 1)
                        lastName = string.Join(" ", names.Skip(1));

                    return new Everens.Speaker()
                    {
                        externalReferenceId = "speaker_" + s.Id,
                        firstname = firstName,
                        lastname = lastName,
                        bio = s.Bio,
                        byline = s.Company,
                        twitter = s.Twitter,
                        github = s.GitHub,
                        imageUrl = s.Images.FirstOrDefault(i => i.ImageType.Slug == "thumb")?.ImageUrl
                    };
                }).ToArray(),
                activities = slots.SelectMany(s => s.RoomSlots).Where(rs => rs.AssignedSession != null).Select(rs =>
                {
                    var s = rs.AssignedSession;
                    
                    return new Everens.Activity()
                    {
                        externalReferenceId = "session_" + rs.AssignedSession.Id,
                        title = s.Name,
                        startTime = Conference.ConvertToUtcDateTime(rs.Slot.Start).ToString("s", CultureInfo.InvariantCulture),
                        endTime = Conference.ConvertToUtcDateTime(rs.Slot.End).ToString("s", CultureInfo.InvariantCulture),
                        description = s.Description,
                        externalPlaceReferenceIds = new[] {"room_" + rs.Room.Id},
                        externalSpeakerReferenceIds = rs.AssignedSession.Speakers.Select(sp => "speaker_" + sp.Speaker.Id).ToArray(),
                        externalTaxonomyReferenceIds = new string[0] 
                    };
                }).ToArray(),
                sponsors = sponsors.Select(s => new Everens.Sponsor
                {
                    externalReferenceId = "sponsor_" + s.Id,
                    externalTaxonomyReferenceId = "sponsors",
                    information = new Everens.Information()
                    {
                        title = s.Name,
                        website = s.Web
                    }
                }).ToArray()
            };
            
            return Json(export, JsonRequestBehavior.AllowGet);
        }

        [Route("{conferenceSlug}/export/speaker-csv")]
        public async Task<ActionResult> SpeakerCsv(string separator)
        {
            var conferenceId = ConferenceId;
            var conference = Conference;
            var speakers = conferenceService.GetSpeakers(conferenceId).ToArray();
            var slots = conferenceService.GetSlotsAndSessions(conferenceId).ToArray();

            var list = new List<SpeakerCsv.Speaker>();
            foreach (var sp in speakers)
            {
                var spSlots = slots.Where(sl => sl.RoomSlots != null && sl.RoomSlots.Any(rs => rs.AssignedSession != null && rs.AssignedSession.Speakers.Any(asp => asp.SpeakerId == sp.Id))).ToArray();

                foreach (var slot in spSlots)
                {
                    var roomSlot = slot.RoomSlots.First(rs => rs.AssignedSession != null && rs.AssignedSession.Speakers.Any(asp => asp.SpeakerId == sp.Id));
                    list.Add(new SpeakerCsv.Speaker()
                    {
                        Name = sp.Name,
                        Company = sp.Company,
                        Twitter = sp.Twitter,
                        Start = slot.Start,
                        End = slot.End,
                        Room = roomSlot.Room?.Name,
                        Session = roomSlot.AssignedSession?.Name
                    });

                }
            }

            var sepChar = ',';
            if (!string.IsNullOrEmpty(separator))
            {
                sepChar = separator[0];
            }
            return new CsvActionResult<SpeakerCsv.Speaker>(list, "Speakers.csv", sepChar);
        }


        protected Conference Conference
        {
            get
            {
                if (conference != null)
                    return conference;
                return conference = dbContext.Conferences.Single(c => c.Slug == ConferenceSlug);
            }

        }
        protected int ConferenceId
        {
            get
            {
                if (conferenceId != 0)
                    return conferenceId;

                return conferenceId = Conference.Id;
            }
        }

        protected string ConferenceSlug
        {
            get
            {
                if (conferenceSlug != null)
                    return conferenceSlug;

                return conferenceSlug = (string)RouteData.Values["conferenceSlug"];
            }
        }

    }
}