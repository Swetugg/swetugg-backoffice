using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Areas.Swetugg2015.Controllers
{
    [RouteArea("Swetugg2015", AreaPrefix = "swetugg-2015")]
    public class ConferenceController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IConferenceService conferenceService;

        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;
        private string appInsightsInstrumentationKey;
        private string facebookAppId;

        public ConferenceController(ApplicationDbContext dbContext)
        {
            this.conferenceSlug = "swetugg-2015";
            this.conferenceService = new CachedConferenceService(new ConferenceService(dbContext));
            this.appInsightsInstrumentationKey = ConfigurationManager.AppSettings["ApplicationInsights.InstrumentationKey"];
            this.facebookAppId = ConfigurationManager.AppSettings["Facebook_Api_AppId"];
        }

        protected override void OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext filterContext)
        {
            ViewData["InstrumentationKey"] = appInsightsInstrumentationKey;
            ViewData["FacebookAppId"] = facebookAppId;
            base.OnResultExecuting(filterContext);
        }

        [Route("")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            var conf = Conference;

            var speakers = conferenceService.GetSpeakers(conf.Id);
            var sessions = conferenceService.GetSessions(conf.Id);
            var slots = conferenceService.GetSlotsAndSessions(conf.Id);
            var rooms = conferenceService.GetRooms(conf.Id);
            var sponsors = conferenceService.GetSponsors(conf.Id);
            var speakerImages = conferenceService.GetSpeakerImages(conf.Id);

            ViewData["Speakers"] = speakers;
            ViewData["Sessions"] = sessions;
            ViewData["Slots"] = slots;
            ViewData["Rooms"] = rooms;
            ViewData["Sponsors"] = sponsors;
            ViewData["Conference"] = conf;
            ViewData["SpeakerImages"] = speakerImages;

            return View();
        }

        [Route("code-of-conduct")]
        public Microsoft.AspNetCore.Mvc.ActionResult CodeOfConduct()
        {
            ViewData["Conference"] = Conference;
            return View();
        }

        [Route("speakers/{speakerSlug}")]
        public Microsoft.AspNetCore.Mvc.ActionResult Speaker(string speakerSlug)
        {
            var speaker = conferenceService.GetSpeakerBySlug(ConferenceId, speakerSlug);
            return View(speaker);
        }

        protected override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
        {
            ViewBag.Conference = Conference;
            base.OnActionExecuted(context);
        }

        protected Conference Conference
        {
            get
            {
                if (conference != null)
                    return conference;

                return conference = conferenceService.GetConferenceBySlug(ConferenceSlug);
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