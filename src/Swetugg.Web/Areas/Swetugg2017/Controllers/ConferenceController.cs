using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Areas.Swetugg2017.Controllers
{
    [RouteArea("Swetugg2017", AreaPrefix = "swetugg-2017")]
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
            this.conferenceSlug = "swetugg-2017";
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
            if (conf == null)
            {
                return HttpNotFound();
            }

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

            bool ticketSalesOpen = false;
            /*
            // Do we have forced open/close info for ticket sales?
            if (!bool.TryParse(ConfigurationManager.AppSettings["Ticket_Sales_Force"], out ticketSalesOpen))
            {
                // No forced info. Let's see if we have an open date/time
                DateTime ticketsOpenDateTime;
                if (DateTime.TryParse(ConfigurationManager.AppSettings["Ticket_Sales_Open_Date"], 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out ticketsOpenDateTime))
                {
                    // Ticket sales should be open if the local conference time is greater than the date/time
                    ticketSalesOpen = conference.CurrentTime() > ticketsOpenDateTime;
                }
            }
            */
            bool conferenceOngoing = false;
            if (conference.Start.HasValue && conference.End.HasValue)
            {
                var currentTime = conference.CurrentTime();
                conferenceOngoing = currentTime > conference.Start.Value.Date &&
                                    currentTime < conference.End.Value.Date.AddDays(1);
            }

            ViewData["TicketSalesOpen"] = ticketSalesOpen;
            ViewData["ConferenceOngoing"] = conferenceOngoing;
            ViewData["TicketUrl"] = ConfigurationManager.AppSettings["Ticket_Url"];
            ViewData["TicketKey"] = ConfigurationManager.AppSettings["Ticket_Key"];
            ViewData["SponsorTicketKey"] = ConfigurationManager.AppSettings["SponsorTicket_Key"];

            return View();
        }

        [Route("now")]
        public Microsoft.AspNetCore.Mvc.ActionResult Now()
        {
            var conf = Conference;
            if (conf == null)
            {
                return HttpNotFound();
            }

            var sponsors = conferenceService.GetSponsors(conf.Id);
            ViewData["Sponsors"] = sponsors;

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
            if (speaker == null)
            {
                return HttpNotFound();
            }

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

                if (Conference == null)
                    return 0;

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