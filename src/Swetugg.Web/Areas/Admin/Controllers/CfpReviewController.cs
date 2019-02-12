using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Controllers;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public static class OrderByQueryableExtensions
    {
        public static IOrderedQueryable<TSource> OrderByDynamic<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> expression, IComparer<TKey> comparer, bool descending)
        {
            if (descending)
                return source.OrderByDescending(expression, comparer);

            return source.OrderBy(expression, comparer);
        }
        public static IOrderedQueryable<TSource> OrderByDynamic<TSource, TKey>(this IQueryable<TSource> source,
            Expression<Func<TSource, TKey>> expression, bool descending)
        {
            if (descending)
                return source.OrderByDescending(expression);

            return source.OrderBy(expression);
        }
    }

    public class CfpReviewController : ConferenceAdminControllerBase
    {
        public CfpReviewController(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        [Route("{conferenceSlug}/cfp")]
        public async Task<ActionResult> Index(string orderBy, bool? descending)
        {
            var conferenceId = ConferenceId;
            var speakersQuery = dbContext.CfpSpeakers
                .Where(s => s.ConferenceId == conferenceId)
                .Include(s => s.Sessions);

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "Name";
            }
            switch (orderBy)
            {
                case "Name":
                    speakersQuery = speakersQuery.OrderByDynamic(s => s.Name, descending.GetValueOrDefault());
                    break;
                case "LastUpdateSpeaker":
                    speakersQuery = speakersQuery.OrderByDynamic(s => s.LastUpdate, descending.GetValueOrDefault());
                    break;
                case "LastUpdateSession":
                    if (descending.GetValueOrDefault())
                    {
                        speakersQuery = speakersQuery.OrderByDescending(s => s.Sessions.Max(se => se.LastUpdate));
                    }
                    else
                    {
                        speakersQuery = speakersQuery.OrderBy(s => s.Sessions.Min(se => se.LastUpdate));
                    }
                    break;
            }

            var speakers = await speakersQuery
                .ToListAsync();

            ViewBag.Conference = Conference;
            ViewBag.OrderByList = new List<Tuple<string, bool, string>>()
            {
                new Tuple<string, bool, string>("Name", false, "Name (A-Z)"),
                new Tuple<string, bool, string>("Name", true, "Name (Z-A)"),
                new Tuple<string, bool, string>("LastUpdateSpeaker", true, "Speaker update"),
                new Tuple<string, bool, string>("LastUpdateSession", true, "Session update"),
            };

            return View(speakers);
        }

        [Route("{conferenceSlug}/cfp/speaker/{id:int}")]
        public async Task<ViewResult> Speaker(int id)
        {
            var conferenceId = ConferenceId;
            var speaker = await dbContext.CfpSpeakers
                .Include(s => s.Sessions.Select(se => se.Session))
                .Include(s => s.Speaker).SingleAsync(s => s.ConferenceId == conferenceId && s.Id == id);

            ViewBag.Conference = Conference;

            return View(speaker);
        }

        [Route("{conferenceSlug}/cfp/session/{id:int}")]
        #pragma warning disable 0108
        public async Task<ViewResult> Session(int id)
        #pragma warning restore 0108
        {
            var conferenceId = ConferenceId;
            var session = await dbContext.CfpSessions
                .Include(s => s.Speaker)
                .Include(s => s.SessionType)
                .Include(s => s.Session).SingleAsync(s => s.ConferenceId == conferenceId && s.Id == id);

            ViewBag.Conference = Conference;
            ViewBag.Speaker = session.Speaker;

            return View(session);
        }

        [HttpPost]
        [Route("{conferenceSlug}/cfp/speaker/{id:int}/promote")]
        [ValidateAntiForgeryToken]
        public async Task<RedirectToRouteResult> Promote(int id)
        {
            var conferenceId = ConferenceId;
            var cfpSpeaker = await dbContext.CfpSpeakers
                .SingleAsync(s => s.ConferenceId == conferenceId && s.Id == id);

            var speaker = new Speaker()
            {
                ConferenceId = conferenceId,
                Name = cfpSpeaker.Name,
                Company = cfpSpeaker.Company,
                Bio = cfpSpeaker.Bio,
                Slug = cfpSpeaker.Name.Slugify(),
                Web = cfpSpeaker.Web,
                Twitter = cfpSpeaker.Twitter,
                GitHub = cfpSpeaker.GitHub
            };
            cfpSpeaker.Speaker = speaker;

            dbContext.Entry(speaker).State = EntityState.Added;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Speaker", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/cfp/speaker/{id:int}/promote-sessions")]
        public async Task<ActionResult> PromoteSessions(int id, List<int> sessionIds)
        {
            var conferenceId = ConferenceId;

            var cfpSpeaker = await dbContext.CfpSpeakers
                .Include(s => s.Sessions.Select(se => se.SessionType))
                .Include(s => s.Speaker.Sessions)
                .SingleAsync(s => s.ConferenceId == conferenceId && s.Id == id);
            var cfpSessions = cfpSpeaker.Sessions;
            
            var speaker = cfpSpeaker.Speaker;

            if (sessionIds != null && sessionIds.Any())
            {
                foreach (var s in cfpSessions.Where(s => sessionIds.Contains(s.Id)))
                {
                    var session = new Session()
                    {
                        ConferenceId = conferenceId,
                        Name = s.Name,
                        Slug = s.Name.Slugify(),
                        Description = s.Description,
                        SessionType = s.SessionType,
                    };
                    s.Session = session;
                    speaker.Sessions.Add(new SessionSpeaker()
                    {
                        Session = session
                    });
                }
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Speaker", new { id });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("{conferenceSlug}/cfp/speaker/{id:int}/update")]
        public async Task<ActionResult> Update(int id)
        {
            var conferenceId = ConferenceId;

            var cfpSpeaker = await dbContext.CfpSpeakers
                .Include(s => s.Sessions.Select(se => se.Session))
                .Include(s => s.Sessions.Select(se => se.SessionType))
                .Include(s => s.Speaker).SingleAsync(s => s.ConferenceId == conferenceId && s.Id == id);
            var cfpSessions = cfpSpeaker.Sessions;

            var speaker = cfpSpeaker.Speaker;
            if (speaker != null)
            {
                speaker.Name = cfpSpeaker.Name;
                speaker.Bio = cfpSpeaker.Bio;
                speaker.Web = cfpSpeaker.Web;
                speaker.Twitter = cfpSpeaker.Twitter;
                speaker.Company = cfpSpeaker.Company;
                speaker.GitHub = cfpSpeaker.GitHub;
            }
            foreach (var cfpSession in cfpSessions)
            {
                var session = cfpSession.Session;
                if (session != null)
                {
                    session.Name = cfpSession.Name;
                    session.Description = cfpSession.Description;
                    session.SessionType = cfpSession.SessionType;
                }
            }
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Speaker", new {id});
        }
    }
}