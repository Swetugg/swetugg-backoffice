using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class CfpSession
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        public int SpeakerId { get; set; }
        public CfpSpeaker Speaker { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Title")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Tags { get; set; }

        [StringLength(250)]
        public string Audience { get; set; }

        [StringLength(250)]
        public string Level { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Comments { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Session type")]
        public SessionType SessionType { get; set; }
        public int? SessionTypeId { get; set; }

        public Session Session { get; set; }
        public int? SessionId { get; set; }

	    public string Notes { get; set; }
	    public string Status { get; set; }
	    public bool? Decided { get; set; }
    }
}