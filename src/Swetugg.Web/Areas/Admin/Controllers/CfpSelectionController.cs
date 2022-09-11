using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class CfpSessionData
    {
        public int Id { get; set; }
        public string Speaker { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string Audience { get; set; }
        public string Description { get; set; }
        public string SessionType { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }
        public bool? IsDecided { get; set; }
        public string Status { get; set; }
        public int SpeakerId { get; set; }
    }

    public class CfpSelectionController : ConferenceAdminControllerBase
    {
        public CfpSelectionController(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        [Route("{conferenceSlug}/cfp/api/sessions")]
        public Microsoft.AspNetCore.Mvc.ActionResult Sessions()
        {
            var l = GetSessions().ToList();
            l.ForEach(s => { if (string.IsNullOrWhiteSpace(s.Status)) s.Status = "-"; });
            return Json(l);

        }

        [Route("{conferenceSlug}/cfp/api/sessions-csv")]
        public Microsoft.AspNetCore.Mvc.ActionResult SessionsCsv()
        {
            var sessions = GetSessions();

            var sb = new StringBuilder();

            sb.AppendLine("Speaker;Title;Tags;Level;SessionType");

            foreach (var session in sessions)
            {
                sb.AppendLine($"{session.Speaker};{session.Title.Replace(';', ',')};{session.Category.Replace(';', ',')};{session.Level.Replace(';', ',')};{session.SessionType}");
            }


            Response.ContentType = "text/csv";

            return Content(sb.ToString());
        }

        private IEnumerable<CfpSessionData> GetSessions()
        {
            var conferenceId = dbContext.Conferences.SingleAsync(c => c.Slug == ConferenceSlug).Id;

            var sessions = dbContext.CfpSpeakers
                .Where(s => s.ConferenceId == conferenceId)
                .Include(s => s.Sessions)
                .SelectMany(s => s.Sessions.Select(e => new CfpSessionData
                {
                    Id = e.Id,
                    Speaker = s.Name,
                    SpeakerId = s.Id,
                    Title = e.Name,
                    Category = e.Category,
                    Description = e.Description,
                    Level = e.Level,
                    Audience = e.Audience,
                    SessionType = e.SessionType.Slug,
                    Notes = e.Notes,
                    IsDecided = e.Decided,
                    Status = e.Status
                }));

            return sessions;
        }

        [Route("{conferenceSlug}/cfp/list")]
        public Microsoft.AspNetCore.Mvc.ActionResult List()
        {
            ViewBag.Conference = Conference;
            return View();
        }

        [ValidateAntiForgeryToken]
        [Route("{conferenceSlug}/cfp/update")]
        [HttpPost]
        public Microsoft.AspNetCore.Mvc.ActionResult Update(CfpSessionData data)
        {
            var sessionToUpdate = dbContext.CfpSessions
                .Include(s => s.Speaker)
                .SingleOrDefault(s => s.Id == data.Id);

            if (sessionToUpdate == null)
            {
                return NotFound();
            }

            var sessionType = dbContext.SessionTypes.SingleOrDefault(st => st.Slug == data.SessionType);

            if (sessionType != null)
                sessionToUpdate.SessionTypeId = sessionType.Id;
            sessionToUpdate.Category = data.Category;
            sessionToUpdate.Notes = data.Notes;
            sessionToUpdate.Status = string.Equals(data.Status, "-") ? null : data.Status;
            sessionToUpdate.Decided = data.IsDecided;

            dbContext.SaveChanges();

            return Json(data);
        }
    }
}
