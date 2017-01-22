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
    public class SpeakerView
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Twitter { get; set; }
        public string Web { get; set; }
        public string GitHub { get; set; }
        public string Bio { get; set; }
    }

    public class SessionView
    {
        public string Name { get; set; }
        public SpeakerView[] Speakers { get; set; }
        public string Room { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
    }

    public class ScheduleApiController : Controller
    {
        private readonly IConferenceService conferenceService;

        private readonly ApplicationDbContext dbContext;
        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;

        public ScheduleApiController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.conferenceService = new CachedConferenceService(new ConferenceService(dbContext));

        }

        [Route("{conferenceSlug}/schedule-feed")]
        public async Task<ActionResult> Index()
        {
            var sessions = await GetSessions();
            var roomSlug = Request.QueryString["room"];
            var dateParam = Request.QueryString["date"];
            DateTime date = DateTime.MinValue;
            if (dateParam != null)
            {
                DateTime.TryParse(dateParam, out date);
            }
            var cultureInfo = CultureInfo.GetCultureInfo("sv-SE");

            var sessionsView = from s in sessions
                where s.RoomSlots.Any()
                let roomSlot = s.RoomSlots.First()
                               where 
                                (roomSlug == null || roomSlot.Room.Slug == roomSlug) &&
                                (date == DateTime.MinValue || roomSlot.Slot.Start.Date == date)
                               orderby roomSlot.Slot.Start, roomSlot.Room.Name
                select new SessionView()
                {
                    Name = s.Name,
                    Description = s.Description,
                    Speakers = s.Speakers.Select(sp => new SpeakerView()
                    {
                        Name = sp.Speaker.Name, 
                        Company = sp.Speaker.Company,
                        Bio = sp.Speaker.Bio,
                        Twitter = sp.Speaker.Twitter != null ? "@" + sp.Speaker.Twitter .Replace("@", string.Empty) : null,
                        Web = sp.Speaker.Web,
                        GitHub = sp.Speaker.GitHub
                    }).ToArray(),
                    StartTime = roomSlot.Slot.Start.ToString(cultureInfo),
                    EndTime = roomSlot.Slot.End.ToString(cultureInfo),
                    Room = roomSlot.Room.Name
                };
            return Json(sessionsView, JsonRequestBehavior.AllowGet);
        }

        [Route("{conferenceSlug}/slots-feed")]
        public ActionResult GetSlots()
        {
            var slots = this.conferenceService.GetSlotsAndSessions(this.ConferenceId);
            var rooms = this.conferenceService.GetRooms(this.ConferenceId);

            var res = from s in slots
                      select new
                      {
                          Start = s.Start.ToString(),
                          End = s.End.ToString(),
                          s.Title,
                          Sessions = from r in s.RoomSlots
                          select new
                          {
                              Room = rooms.Where(room => room.Id == r.RoomId).FirstOrDefault().Name,
                              Name = r.AssignedSession == null ? null : r.AssignedSession.Name,
                              Description = r.AssignedSession == null ? null : r.AssignedSession.Description,
                              Speakers = r.AssignedSession == null ? null :
                                from speaker in r.AssignedSession.Speakers
                                         select new
                                         {
                                             speaker.Speaker.Name
                                         }
                          }
                      };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<Session>> GetSessions()
        {
            var conferenceId = ConferenceId;
            var sessions = await dbContext.Sessions.
                Include(s => s.Speakers.Select(sp => sp.Speaker)).
                Include(s => s.RoomSlots.Select(rs => rs.Room)).
                Include(s => s.RoomSlots.Select(rs => rs.Slot)).
                Where(s => s.ConferenceId == conferenceId).ToListAsync();
            return sessions;
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