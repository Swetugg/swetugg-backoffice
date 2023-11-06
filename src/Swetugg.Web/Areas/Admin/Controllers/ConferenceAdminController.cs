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
        private readonly ApplicationDbContext _dbContext;

        public ConferenceAdminController(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [OverrideAuthorization()]
        [Authorize()]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var conferences = await _dbContext.Conferences.OrderByDescending(c => c.Start).ToListAsync();
            return View(conferences);
        }

        [OverrideAuthorization()]
        [Authorize()]
        [Route("{id:int}")]
        public async Task<ActionResult> Conference(int id)
        {
            var conferences = await _dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(conferences);
        }

        [Route("edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var conference = await _dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(conference);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Conference conference)
        {
            if (ModelState.IsValid)
            {
                try
                {
					_dbContext.Entry(conference).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();

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
            var conference = await _dbContext.Conferences.SingleAsync(c => c.Id == id);
            return View(new ConferenceImportModel() { Conference = conference});
        }

        [Route("new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("new", Order = 2)]
        public async Task<ActionResult> Edit(Conference conference)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Conferences.Add(conference);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Conference", "ConferenceAdmin", new { conference.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(conference);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var conf = await _dbContext.Conferences.SingleOrDefaultAsync(c => c.Id == id);

            var slots = await _dbContext.Slots.Where(s => s.ConferenceId == id).Include(rs => rs.RoomSlots).ToListAsync();
            // Delete Schedule
            foreach (var slot in slots)
            {
                foreach (var roomSlot in slot.RoomSlots.ToArray())
                {
                    _dbContext.Entry(roomSlot).State = EntityState.Deleted;
                }
                _dbContext.Entry(slot).State = EntityState.Deleted;
            }

            // Delete Rooms
            var rooms = await _dbContext.Rooms.Where(r => r.ConferenceId == id).ToListAsync();
            foreach (var room in rooms)
            {
                _dbContext.Entry(room).State=EntityState.Deleted;
            }

            // Delete Sessions
            var sessions = await _dbContext.Sessions.Where(s => s.ConferenceId == id).Include(s => s.Speakers).ToListAsync();
            foreach (var session in sessions)
            {
                foreach (var speaker in session.Speakers.ToArray())
                {
                    _dbContext.Entry(speaker).State=EntityState.Deleted;
                }
                _dbContext.Entry(session).State=EntityState.Deleted;
            }

            // Delete Speakers
            var speakers = await _dbContext.Speakers.Where(s => s.ConferenceId == id).ToListAsync();
            foreach (var speaker in speakers)
            {
                // Delete speaker images
                foreach (var image in speaker.Images.ToArray())
                {
                    _dbContext.Entry(image).State = EntityState.Deleted;
                }

                _dbContext.Entry(speaker).State = EntityState.Deleted;
            }

            // Delete Image types
            var imageTypes = await _dbContext.ImageTypes.Where(r => r.ConferenceId == id).ToListAsync();
            foreach (var imageType in imageTypes)
            {
                _dbContext.Entry(imageType).State = EntityState.Deleted;
            }

            // Delete Sponsors
            var sponsors = await _dbContext.Sponsors.Where(s => s.ConferenceId == id).ToListAsync();
            foreach (var sponsor in sponsors)
            {
                _dbContext.Entry(sponsor).State = EntityState.Deleted;
            }

            _dbContext.Entry(conf).State = EntityState.Deleted;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}