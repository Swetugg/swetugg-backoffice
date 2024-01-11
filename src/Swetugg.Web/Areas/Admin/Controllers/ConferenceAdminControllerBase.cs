﻿using System.Linq;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "ConferenceManager")]
    [RouteArea("Admin", AreaPrefix = "admin")]
    public class ConferenceAdminControllerBase : Controller
    {
        protected ApplicationDbContext dbContext;

        private int conferenceId;
        private string conferenceSlug;
        private Conference conference;

        public ConferenceAdminControllerBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        protected override void OnActionExecuted(ActionExecutedContext context)
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
        public int ConferenceId
        {
            get
            {
                if (conferenceId != 0)
                    return conferenceId;

                return conferenceId = Conference.Id;
            }
            set
            {
                conferenceId = value;
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