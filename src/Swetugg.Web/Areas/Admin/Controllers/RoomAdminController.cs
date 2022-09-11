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
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index()
        {
            var conferenceId = ConferenceId;
            var rooms = from r in dbContext.Rooms
                        where r.ConferenceId == conferenceId
                        orderby r.Priority
                        select r;
            var roomsList = await rooms.ToListAsync();

            return View(roomsList);
        }

		[Route("{conferenceSlug}/rooms/edit/{id:int}", Order = 1)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id)
        {
            var room = await dbContext.Rooms.SingleAsync(s => s.Id == id);
            return View(room);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/rooms/edit/{id:int}", Order = 1)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(int id, Room room)
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
        public Microsoft.AspNetCore.Mvc.ActionResult Edit()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/rooms/new", Order = 2)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Edit(Room room)
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