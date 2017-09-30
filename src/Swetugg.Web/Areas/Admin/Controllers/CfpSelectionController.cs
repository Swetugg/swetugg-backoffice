using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
	public class CfpSessionData
	{
		public int Id { get; set; }
		public string Speaker { get; set; }
		public string Title { get; set; }
		public string Tags { get; set; }
		public string Level { get; set; }
		public string Audience { get; set; }
		public string Description { get; set; }
		public string SessionType { get; set; }
		public string Notes { get; set; }
		public bool? IsDecided { get; set; }
		public string Status { get; set; }
		public int SpeakerId { get; set; }
	}

	public class CfpSelectionController : ConferenceAdminControllerBase
	{
		public CfpSelectionController(ApplicationDbContext dbContext): base(dbContext)
		{
		}

		[Route("{conferenceSlug}/cfp/api/sessions")]
		public ActionResult Index()
		{
			var conferenceId = dbContext.Conferences.Single(c => c.Slug == ConferenceSlug).Id;

			var sessions = dbContext.CfpSpeakers
				.Where(s => s.ConferenceId == conferenceId)
				.Include(s => s.Sessions)
				.SelectMany(s => s.Sessions.Select(e => new CfpSessionData
				{
					Id = e.Id,
					Speaker = s.Name,
					SpeakerId = s.Id,
					Title = e.Name,
					Tags = e.Tags,
					Description = e.Description,
					Level = e.Level,
					Audience = e.Audience,
					SessionType = e.SessionType.Slug,
					Notes = e.Notes,
					IsDecided = e.Decided,
					Status = e.Status
				}));
				

			return Json(sessions, JsonRequestBehavior.AllowGet);

		}

		[Route("{conferenceSlug}/cfp/list")]
		public ActionResult List()
		{
			return View();
		}

		[Route("{conferenceSlug}/cfp/update")]
		[HttpPost]
		public ActionResult Update(CfpSessionData data)
		{
			var sessionToUpdate = dbContext.CfpSessions
				.Include(s => s.Speaker)
				.SingleOrDefault(s => s.Id == data.Id);

			if (sessionToUpdate == null)
			{
				return HttpNotFound();
			}

			sessionToUpdate.Notes = data.Notes;
			sessionToUpdate.Status = data.Status;
			sessionToUpdate.Decided = data.IsDecided;

			dbContext.SaveChanges();

			return Json(data);
		}
	}
}
