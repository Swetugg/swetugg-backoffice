using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class RoomSlot
    {
        public int RoomId { get; set; }
        public int SlotId { get; set; }

        public Room Room { get; set; }
        public Slot Slot { get; set; }

        public int? AssignedSessionId { get; set; }
        public Session AssignedSession { get; set; }

        public bool IsChanged { get; set; }
    }
}