using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
	public class SpeakerLogisticsModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Company { get; set; }
		public string Email { get; set; }
		public string Twitter { get; set; }
		public bool? Published { get; set; }
		public bool? NeedAccomodation { get; set; }
		public bool? NeedTravel { get; set; }
		public string Notes { get; set; }
		public bool? AttendingDinner { get; set; }
		public bool? TwitterList { get; set; }
		public bool? AccomodationDone { get; set; }
		public bool? TravelDone { get; set; }
	}
	public class SpeakerLogisticsController : ConferenceAdminControllerBase
	{
		public SpeakerLogisticsController(ApplicationDbContext dbContext) : base(dbContext)
		{
		}

		[Route("{conferenceSlug}/speaker-logistics/list")]
		public Microsoft.AspNetCore.Mvc.ActionResult List()
		{
			ViewBag.Conference = Conference;
			return View();
		}

		[Route("{conferenceSlug}/api/speaker-logistics")]
		public Microsoft.AspNetCore.Mvc.ActionResult Speakers()
		{
			var speakers = dbContext.Speakers
				.Include("CfpSpeakers")
				.Include("SpeakerLogistics");

			var speakerLogistics = from s in speakers
								   let cfpSpeaker = s.CfpSpeakers.FirstOrDefault()
								   let logistics = s.SpeakerLogistics.FirstOrDefault()
								   select new SpeakerLogisticsModel
								   {
									   Id = s.Id,
									   Name = s.Name,
									   Company = s.Company,
									   Email = cfpSpeaker.Email,
									   Twitter = cfpSpeaker.Twitter,
									   Published = s.Published,
									   NeedAccomodation = cfpSpeaker.NeedAccommodation,
									   NeedTravel = cfpSpeaker.NeedTravel,
									   Notes = logistics.Notes,
									   AttendingDinner = logistics.AttendingDinner,
									   TwitterList = logistics.TwitterList,
									   AccomodationDone = logistics.AccomodationDone,
									   TravelDone = logistics.TravelDone
								   };

			return Json(speakerLogistics);
		}
	}
}
