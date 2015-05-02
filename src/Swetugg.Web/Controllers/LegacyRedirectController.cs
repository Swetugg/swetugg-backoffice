using System.Linq;
using System.Web.Mvc;
using Swetugg.Web.Models;
using Swetugg.Web.Services;

namespace Swetugg.Web.Controllers
{
    public class LegacyRedirectController : Controller
    {
        private ApplicationDbContext _dbContext;

        public LegacyRedirectController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("Speakers/Details/{id}")]
        public ActionResult SpeakerDetails(int id)
        {
            // Quick hack to get ID:s correct from imported data.
            if (id > 3)
                id--;
            if (id > 40)
                id--;
            var speaker = _dbContext.Speakers.SingleOrDefault(s => s.Id == id);
            if (speaker == null)
            {
                return RedirectPermanent("/");
            }

            return RedirectToActionPermanent("Speaker", "Conference", new { Area = "Swetugg2015", speakerSlug = speaker.Slug});
        }
        
    }
}