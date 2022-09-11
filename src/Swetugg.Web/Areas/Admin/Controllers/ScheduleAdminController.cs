using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swetugg.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Swetugg.Web.Areas.Admin.Controllers
{
    public class ScheduleViewModel
    {
        public List<Room> Rooms { get; set; }
        public List<Slot> Slots { get; set; }
        public List<Session> UnplacedSessions { get; set; }
        public Conference Conference { get; set; }
        public Slot NewSlot { get; set; }
        public Slot EditSlot { get; set; }
    }

    public class ScheduleAdminController : ConferenceAdminControllerBase
    {
        public ScheduleAdminController(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

		[Route("{conferenceSlug}/schedule")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(int? slotId)
        {
            var allRooms = await GetAllRooms();
            var unplacedSessions = await GetUnplacedSessions();
            var slotsWithSessions = await GetSlotsWithSessions();

            Slot editSlot = null;
            if (slotId != null)
            {
                editSlot = slotId != null ? slotsWithSessions.SingleOrDefault(s => s.Id == slotId) : null;
            }
            return View(new ScheduleViewModel() { Conference = Conference, Rooms = allRooms, Slots = slotsWithSessions, UnplacedSessions = unplacedSessions, EditSlot = editSlot, NewSlot = new Slot()});
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/schedule/slot/edit/{id:int}", Order = 1)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Slot(int id, DateTime day, Slot slot)
        {
            slot.Start = day.Date + slot.Start.TimeOfDay;
            slot.End = day.Date + slot.End.TimeOfDay;
            if (ModelState.IsValid)
            {
                try
                {
                    slot.ConferenceId = ConferenceId;
                    
					dbContext.Entry(slot).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            var model = new ScheduleViewModel()
            {
                Conference = Conference,
                Rooms = await GetAllRooms(),
                Slots = await GetSlotsWithSessions(),
                UnplacedSessions = await GetUnplacedSessions(),
                EditSlot = slot
            };
            return View("Index", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/schedule/slot/new", Order = 2)]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Slot(DateTime day, Slot slot)
        {
            slot.Start = day.Date + slot.Start.TimeOfDay;
            slot.End = day.Date + slot.End.TimeOfDay;
            if (ModelState.IsValid)
            {
                try
                {
                    slot.ConferenceId = ConferenceId;
                    dbContext.Slots.Add(slot);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Updating", ex);
                }
            }
            var model = new ScheduleViewModel()
            {
                Conference = Conference,
                Rooms = await GetAllRooms(),
                Slots = await GetSlotsWithSessions(),
                UnplacedSessions = await GetUnplacedSessions(),
                NewSlot = slot
            };
            return View("Index", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/schedule/slot/{id:int}/delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DeleteSlot(int id)
        {
            var slot = await dbContext.Slots.Include(s => s.RoomSlots).SingleAsync(s => s.Id == id);
            foreach (var roomSlot in slot.RoomSlots)
            {
				dbContext.Entry(roomSlot).State = EntityState.Deleted;
            }
			dbContext.Entry(slot).State = EntityState.Deleted;

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/schedule/slot/{slotId:int}/room/{roomId:int}/assign")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> AssignRoomSlot(int slotId, int roomId, RoomSlot roomSlot)
        {
            if (ModelState.IsValid)
            {
                var oldRoomSlot =
                    await dbContext.RoomSlots.SingleOrDefaultAsync(rs => rs.RoomId == roomId && rs.SlotId == slotId);
                if (oldRoomSlot != null)
                {
                    oldRoomSlot.AssignedSessionId = roomSlot.AssignedSessionId;
					dbContext.Entry(oldRoomSlot).State = EntityState.Modified;
                }
                else
                {
                    dbContext.RoomSlots.Add(roomSlot);
                }
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
		[Route("{conferenceSlug}/schedule/slot/{slotId:int}/room/{roomId:int}/delete")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DeleteRoomSlot(int slotId, int roomId)
        {
            var roomSlot =
                await dbContext.RoomSlots.SingleOrDefaultAsync(rs => rs.RoomId == roomId && rs.SlotId == slotId);

            if (roomSlot != null)
            {
                dbContext.RoomSlots.Remove(roomSlot);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        private async Task<List<Slot>> GetSlotsWithSessions()
        {
            var conferenceId = ConferenceId;
            var slotsWithSessions =
                await
                    dbContext.Slots
                        .Include(s => s.RoomSlots.Select(rs => rs.AssignedSession))
                        .Where(sl => sl.ConferenceId == conferenceId)
                        .OrderBy(s => s.Start)
                        .ToListAsync();
            return slotsWithSessions;
        }

        private async Task<List<Room>> GetAllRooms()
        {
            var conferenceId = ConferenceId;
            var allRooms = await dbContext.Rooms.Where(r => r.ConferenceId == conferenceId).OrderBy(r => r.Priority).ToListAsync();
            return allRooms;
        }

        private async Task<List<Session>> GetUnplacedSessions()
        {
            var conferenceId = ConferenceId;
            var sessions = 
                await 
                    dbContext.Sessions
                    .Include(s => s.Speakers.Select(sp => sp.Speaker))
                    .Include(s => s.RoomSlots)
                    .Where(s => s.ConferenceId == conferenceId && s.RoomSlots.Count == 0)
                    .OrderBy(s => s.Speakers.FirstOrDefault().Speaker.Name)
                    .ToListAsync();
            return sessions;
        }
    }
}
