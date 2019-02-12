using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class SessionAdminController : ConferenceAdminControllerBase
    {

        public SessionAdminController(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        [Route("{conferenceSlug}/sessions")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var sessions = from s in dbContext.Sessions
                where s.ConferenceId == conferenceId
                orderby s.Name
                select s;
            var sessionsList = await sessions.ToListAsync();
            return View(sessionsList);
        }

        [Route("{conferenceSlug}/sessions/{id:int}")]
        #pragma warning disable 0108
        public async Task<ActionResult> Session(int id)
        #pragma warning restore 0108
        {
            var conferenceId = ConferenceId;

            var session = await dbContext.Sessions
                .Include(m => m.Speakers.Select(s => s.Speaker))
                .Include(s => s.Tags)
                .Include(s => s.SessionType)
                .SingleAsync(s => s.Id == id);

            var speakers = await dbContext.Speakers.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Name).ToListAsync();
            var tags = await dbContext.Tags.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Name).ToListAsync();

            ViewBag.Speakers = speakers.Where(s => session.Speakers.All(se => se.SpeakerId != s.Id)).ToList();
            ViewBag.Tags = tags.Where(t => session.Tags.All(st => st.Id != t.Id)).ToList();

            return View(session);
        }

        [Route("{conferenceSlug}/sessions/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var conferenceId = ConferenceId;
            var session = await dbContext.Sessions
                .Include(s => s.SessionType)
                .SingleAsync(s => s.Id == id);
            ViewBag.SessionTypes = dbContext.SessionTypes.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToList();
            return View(session);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Session session)
        {
            var conferenceId = ConferenceId;
            if (ModelState.IsValid)
            {
                try
                {
                    session.ConferenceId = conferenceId;
                    dbContext.Entry(session).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Session", new { session.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
                ViewBag.SessionTypes = dbContext.SessionTypes.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToList();
            }
            return View(session);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/addspeaker/{id:int}")]
        public async Task<ActionResult> AddSpeaker(int id, int speakerId)
        {
            var session = await dbContext.Sessions.Include(m => m.Speakers).SingleAsync(s => s.Id == id);
            session.Speakers.Add(new SessionSpeaker() { SpeakerId = speakerId });
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Session", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/removespeaker/{id:int}")]
        public async Task<ActionResult> RemoveSpeaker(int id, int speakerId)
        {
            var session = await dbContext.Sessions.Include(m => m.Speakers).Where(s => s.Id == id).SingleAsync();
            var sessionSpeaker = session.Speakers.Single(s => s.SpeakerId == speakerId);

            dbContext.Entry(sessionSpeaker).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Session", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/addtag/{id:int}")]
        public async Task<ActionResult> AddTag(int id, int tagId)
        {
            var session = await dbContext.Sessions.Include(m => m.Tags).SingleAsync(s => s.Id == id);
            var tag = await dbContext.Tags.FirstAsync(t => t.Id == tagId);
            session.Tags.Add(tag);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Session", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/removetag/{id:int}")]
        public async Task<ActionResult> RemoveTag(int id, int tagId)
        {
            var session = await dbContext.Sessions.Include(m => m.Tags).Where(s => s.Id == id).SingleAsync();
            
            foreach (var tag in session.Tags)
            {
                if (tag.Id == tagId)
                {
                    session.Tags.Remove(tag);
                    break;
                }
            }

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Session", new { id });
        }

        [Route("{conferenceSlug}/sessions/new", Order = 2)]
        public async Task<ActionResult> Edit()
        {
            var conferenceId = ConferenceId;
            ViewBag.SessionTypes = await dbContext.SessionTypes.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToListAsync();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/new", Order = 2)]
        public async Task<ActionResult> Edit(Session session)
        {
            var conferenceId = ConferenceId;
            if (ModelState.IsValid)
            {
                try
                {
                    session.ConferenceId = conferenceId;
                    dbContext.Sessions.Add(session);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Session", new { session.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
                ViewBag.SessionTypes = await dbContext.SessionTypes.Where(m => m.ConferenceId == conferenceId).OrderBy(s => s.Priority).ToListAsync();
            }
            return View(session);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/sessions/delete/{id:int}", Order = 1)]
        public async Task<ActionResult> Delete(int id)
        {
            var session = await dbContext.Sessions.Include(s => s.Speakers).Include(s => s.RoomSlots).Include(s => s.CfpSessions).SingleAsync(s => s.Id == id);
            
            foreach (var speaker in session.Speakers.ToArray())
            {
                dbContext.Entry(speaker).State = EntityState.Deleted;
            }
            
            foreach (var roomSlot in session.RoomSlots.ToArray())
            {
                dbContext.Entry(roomSlot).State = EntityState.Deleted;
            }
            session.CfpSessions.Clear();

            dbContext.Entry(session).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}