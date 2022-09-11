using Swetugg.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Swetugg.Web.Controllers
{
    public class NowController : Microsoft.AspNetCore.Mvc.Controller
    {
        // GET: Now
        [Route("{conferenceSlug}/now-feed")]
        [HttpGet]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> GetNowFeed()
        {
            await Task.Yield();

            var data = new NowData() { Data = "Hello" };
            return Json(data);
        }

        private string conferenceSlug;

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