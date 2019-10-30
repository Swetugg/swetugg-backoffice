using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        public int? SessionizeId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [StringLength(500)]
        public string VideoUrl { get; set; }

        public bool VideoPublished { get; set; }

        public bool Published { get; set; }

        public int Priority { get; set; }

        public SessionType SessionType { get; set; }
        public int? SessionTypeId { get; set; }

        public ICollection<SessionSpeaker> Speakers { get; set; }

        public ICollection<RoomSlot> RoomSlots { get; set; }
        public ICollection<CfpSession> CfpSessions { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}