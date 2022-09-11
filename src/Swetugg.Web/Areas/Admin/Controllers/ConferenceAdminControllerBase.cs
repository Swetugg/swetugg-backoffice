using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "ConferenceManager")]
    [Area("Admin")]
    public class ConferenceAdminControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        protected ApplicationDbContext dbContext;

        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;

        public ConferenceAdminControllerBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context)
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