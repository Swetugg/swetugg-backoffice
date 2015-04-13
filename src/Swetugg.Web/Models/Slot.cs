using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Swetugg.Web.Models
{
    public class Slot
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [Required]
        public DateTime Start { get; set; }
        
        [Required]
        public DateTime End { get; set; }

        public ICollection<RoomSlot> RoomSlots { get; set; }
    }
}