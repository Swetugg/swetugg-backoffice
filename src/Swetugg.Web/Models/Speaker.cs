using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Swetugg.Web.Areas.Admin.Controllers;

namespace Swetugg.Web.Models
{
    public class Speaker
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        public Guid? SessionizeId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }

        [StringLength(250)]
        public string Company { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Bio { get; set; }

        [StringLength(250)]
        public string Web { get; set; }

        [StringLength(50)]
        public string Twitter { get; set; }

        [StringLength(250)]
        public string GitHub { get; set; }

        public bool Published { get; set; }

        public int Priority { get; set; }

        [StringLength(1000)]
        public string SessionizeImageUrl { get; set; }

        public ICollection<SessionSpeaker> Sessions { get; set; }

        public ICollection<SpeakerImage> Images { get; set; }

        public ICollection<CfpSpeaker> CfpSpeakers { get; set; }

	    public ICollection<SpeakerLogistics> SpeakerLogistics { get; set; }
    }
}