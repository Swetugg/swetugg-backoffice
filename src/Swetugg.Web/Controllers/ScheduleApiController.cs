using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Controllers
{
    public class SessionView
    {
        public string Name { get; set; }
        public string[] Speakers { get; set; }
        public string Room { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
    }

    public class ScheduleApiController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;

        public ScheduleApiController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [Route("{conferenceSlug}/schedule-feed")]
        public async Task<ActionResult> Index()
        {
            var sessions = await GetSessions();

            var cultureInfo = CultureInfo.GetCultureInfo("sv-SE");

            var sessionsView = from s in sessions
                where s.RoomSlots.Any()
                let roomSlot = s.RoomSlots.First()
                               orderby roomSlot.Slot.Start, roomSlot.Room.Name
                select new SessionView()
                {
                    Name = s.Name,
                    Description = s.Description,
                    Speakers = s.Speakers.Select(sp => sp.Speaker.Name).ToArray(),
                    StartTime = roomSlot.Slot.Start.ToString(cultureInfo),
                    EndTime = roomSlot.Slot.End.ToString(cultureInfo),
                    Room = roomSlot.Room.Name
                };
            return Json(sessionsView, JsonRequestBehavior.AllowGet);
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