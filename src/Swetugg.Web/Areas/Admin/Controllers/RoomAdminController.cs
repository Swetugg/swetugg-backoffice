using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Swetugg.Web.Models;

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class RoomAdminController : ConferenceAdminControllerBase
    {
        public RoomAdminController(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        [Route("{conferenceSlug}/rooms")]
        public async Task<ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var rooms = from s in dbContext.Rooms
                        where s.ConferenceId == conferenceId
                           select s;
            var roomsList = await rooms.ToListAsync();

            return View(roomsList);
        }

		[Route("{conferenceSlug}/rooms/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id)
        {
            var room = await dbContext.Rooms.SingleAsync(s => s.Id == id);
            return View(room);
        }

        [HttpPost]
		[Route("{conferenceSlug}/rooms/edit/{id:int}", Order = 1)]
        public async Task<ActionResult> Edit(int id, Room room)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    room.ConferenceId = ConferenceId;
					dbContext.Entry(room).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(room);
        }

		[Route("{conferenceSlug}/rooms/new", Order = 2)]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
		[Route("{conferenceSlug}/rooms/new", Order = 2)]
        public async Task<ActionResult> Edit(Room room)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    room.ConferenceId = ConferenceId;
                    dbContext.Rooms.Add(room);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            return View(room);
        }

    }
}