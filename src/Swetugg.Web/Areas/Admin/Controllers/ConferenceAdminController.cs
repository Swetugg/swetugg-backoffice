using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dapper;
using Swetugg.Web.Controllers;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class ConferenceImportModel
    {
        public Conference Conference { get; set; }
        public string ConnectionString { get; set; }
    }

    [RequireHttps]
    [Authorize(Roles = "ConferenceManager")]
    [RouteArea("Admin", AreaPrefix = "admin")]
    [RoutePrefix("conferences")]
    public class ConferenceAdminController : Controller
    {
        private ApplicationDbContext dbContext;

        public ConferenceAdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [OverrideAuthorization()]
        [Authorize()]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var conferences = await dbContext.Conferences.OrderByDescending(c => c.Start).ToListAsync();
            return View(conferences);
        }

        [OverrideAuthorization()]
        [Authorize()]
        [Route("{id:int}")]
        public async Task<ActionResult> Conference(int id)
        {
            var conferences = await dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(conferences);
        }

        [Route("edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var conference = await dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(conference);
        }

        [HttpPost]
        [Route("edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Conference conference)
        {
            if (ModelState.IsValid)
            {
                try
                {
					dbContext.Entry(conference).State = EntityState.Modified;
					await dbContext.SaveChangesAsync();

                    return RedirectToAction("Conference", "ConferenceAdmin", new { id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(conference);
        }

        [HttpGet]
        [Route("import/{id:int}")]
        public async Task<ActionResult> Import(int id)
        {
            var conference = await dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(new ConferenceImportModel() { Conference = conference});
        }

        [HttpPost]
        [Route("import/{id:int}")]
        public async Task<ActionResult> Import(int id, ConferenceImportModel conferenceImportModel)
        {
            using (var oldSwetugg = new SqlConnection(conferenceImportModel.ConnectionString))
            {
                var oldSponsors = oldSwetugg.Query("SELECT * FROM Sponsors");
                foreach (var oldSponsor in oldSponsors)
                {
                    var sponsor = new Sponsor()
                    {
                        ConferenceId = id,
                        Name = oldSponsor.Name,
                        Description = oldSponsor.Description,
                        Web = oldSponsor.Web,
                        Twitter = oldSponsor.Twitter,
                        Priority = oldSponsor.Priority,
                        Published = oldSponsor.Published,
                        Slug = ((string)oldSponsor.Name).Slugify()
                    };
                    dbContext.Sponsors.Add(sponsor);
                }
                await dbContext.SaveChangesAsync();

                var oldSpeakers = oldSwetugg.Query("SELECT * FROM Speakers");
                foreach (var oldSpeaker in oldSpeakers)
                {
                    var speaker = new Speaker()
                    {
                        ConferenceId = id,
                        Name = oldSpeaker.Name,
                        Company = oldSpeaker.Company,
                        Bio = oldSpeaker.Bio,
                        GitHub = oldSpeaker.GitHub,
                        Twitter = oldSpeaker.Twitter,
                        Priority = oldSpeaker.Priority,
                        Published = oldSpeaker.Published,
                        Slug = ((string)oldSpeaker.Name).Slugify()
                    };
                    dbContext.Speakers.Add(speaker);
                }
                await dbContext.SaveChangesAsync();

                var oldSessions = oldSwetugg.Query(
                    "SELECT s.Id, s.Name, s.Description, s.EmbeddedVideoLink, s.Priority, s.Deleted, ssp.SpeakerId, sp.Name AS SpeakerName " +
                    "FROM Sessions AS s LEFT OUTER JOIN SessionSpeakers AS ssp " +
                    "ON s.Id = ssp.SessionId " +
                    "JOIN Speakers AS sp ON ssp.SpeakerId = sp.Id " +
                    "ORDER BY s.Id");
                int lastId = 0;
                Session session = null;
                foreach (var oldSession in oldSessions)
                {
                    if (lastId != oldSession.Id)
                    {
                        lastId = oldSession.Id;
                        session = new Session()
                        {
                            ConferenceId = id,
                            Name = oldSession.Name,
                            Description = oldSession.Description,
                            VideoUrl = oldSession.EmbeddedVideoLink,
                            VideoPublished = !string.IsNullOrEmpty(oldSession.EmbeddedVideoLink),

                            Priority = oldSession.Priority,
                            Published = !(oldSession.Deleted),
                            Speakers = new List<SessionSpeaker>(),
                            Slug = ((string)oldSession.Name).Slugify()
                        };
                        dbContext.Sessions.Add(session);
                    }
                    if (oldSession.SpeakerId != null && session != null)
                    {
                        var speakerSlug = ((string) oldSession.SpeakerName).Slugify();
                        var speaker = await dbContext.Speakers.SingleOrDefaultAsync(s => s.Slug == speakerSlug);
                        if (speaker != null)
                        {
                            session.Speakers.Add(new SessionSpeaker() { Session = session, Speaker = speaker});
                        }
                    }
                }
                await dbContext.SaveChangesAsync();

                var oldRooms = oldSwetugg.Query("SELECT * FROM Rooms");
                foreach (var oldRoom in oldRooms)
                {
                    var room = new Room()
                    {
                        ConferenceId = id,
                        Name = oldRoom.Name,
                        Description = oldRoom.Description,
                        Priority = oldRoom.Priority,
                        Slug = ((string)oldRoom.Name).Slugify()
                    };
                    dbContext.Rooms.Add(room);
                }
                await dbContext.SaveChangesAsync();

                var oldSlots = oldSwetugg.Query("SELECT * FROM Slots");
                foreach (var oldSlot in oldSlots)
                {
                    var slot = new Slot()
                    {
                        ConferenceId = id,
                        Start = oldSlot.Start,
                        End = oldSlot.End,
                        Title = oldSlot.Description
                    };
                    dbContext.Slots.Add(slot);
                }
                await dbContext.SaveChangesAsync();

                var allRooms = await dbContext.Rooms.Where(r => r.ConferenceId == id).ToListAsync();
                var allSessions = await dbContext.Sessions.Where(r => r.ConferenceId == id).ToListAsync();
                foreach (var slot in await dbContext.Slots.Where(s => s.ConferenceId == id).ToListAsync())
                {
                    foreach (var room in allRooms)
                    {
                        var oldRoomSlot = oldSwetugg.Query("SELECT rs.SessionId, se.Id, se.Name " +
                                                           "FROM RoomSlots rs " +
                                                           "JOIN Slots s ON rs.SlotId = s.Id " +
                                                           "JOIN Rooms r ON rs.RoomId = r.Id " +
                                                           "JOIN Sessions se ON se.Id = rs.SessionId " +
                                                           "WHERE s.Start = @SlotStart " +
                                                           "AND r.Name = @RoomName", 
                                                           new { SlotStart = slot.Start, RoomName = room.Name}).SingleOrDefault();
                        if (oldRoomSlot != null)
                        {
                            var sessionSlug = ((string) oldRoomSlot.Name).Slugify();
                            session = allSessions.SingleOrDefault(s => s.Slug == sessionSlug);
                            if (slot.RoomSlots == null)
                            {
                                slot.RoomSlots = new List<RoomSlot>();
                            }
                            slot.RoomSlots.Add(new RoomSlot()
                            {
                                Room = room,
                                Slot = slot,
                                AssignedSession = session
                            });
                        }
                    }
                }
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Conference", new {id});

        }

        [Route("new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [Route("new", Order = 2)]
        public async Task<ActionResult> Edit(Conference conference)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    dbContext.Conferences.Add(conference);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Conference", "ConferenceAdmin", new { conference.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(conference);
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var conf = await dbContext.Conferences.SingleOrDefaultAsync(c => c.Id == id);

            var slots = await dbContext.Slots.Where(s => s.ConferenceId == id).Include(rs => rs.RoomSlots).ToListAsync();
            // Delete Schedule
            foreach (var slot in slots)
            {
                foreach (var roomSlot in slot.RoomSlots.ToArray())
                {
                    dbContext.Entry(roomSlot).State = EntityState.Deleted;
                }
                dbContext.Entry(slot).State = EntityState.Deleted;
            }

            // Delete Rooms
            var rooms = await dbContext.Rooms.Where(r => r.ConferenceId == id).ToListAsync();
            foreach (var room in rooms)
            {
                dbContext.Entry(room).State=EntityState.Deleted;
            }

            // Delete Sessions
            var sessions = await dbContext.Sessions.Where(s => s.ConferenceId == id).Include(s => s.Speakers).ToListAsync();
            foreach (var session in sessions)
            {
                foreach (var speaker in session.Speakers.ToArray())
                {
                    dbContext.Entry(speaker).State=EntityState.Deleted;
                }
                dbContext.Entry(session).State=EntityState.Deleted;
            }

            // Delete Speakers
            var speakers = await dbContext.Speakers.Where(s => s.ConferenceId == id).ToListAsync();
            foreach (var speaker in speakers)
            {
                dbContext.Entry(speaker).State = EntityState.Deleted;
            }

            // Delete Sponsors
            var sponsors = await dbContext.Sponsors.Where(s => s.ConferenceId == id).ToListAsync();
            foreach (var sponsor in sponsors)
            {
                dbContext.Entry(sponsor).State = EntityState.Deleted;
            }

            // Delete Cfp
            var cfpSpeakers = await dbContext.CfpSpeakers.Where(s => s.ConferenceId == id).Include(s => s.Sessions).ToListAsync();
            foreach (var speaker in cfpSpeakers)
            {
                foreach (var session in speaker.Sessions.ToArray())
                {
                    dbContext.Entry(session).State = EntityState.Deleted;
                }
                dbContext.Entry(speaker).State = EntityState.Deleted;
            }

            dbContext.Entry(conf).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}