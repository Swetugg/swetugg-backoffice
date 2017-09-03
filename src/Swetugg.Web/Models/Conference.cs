using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class Conference
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
               ApplyFormatInEditMode = true)]
        public DateTime? Start { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
               ApplyFormatInEditMode = true)]
        public DateTime? End { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
               ApplyFormatInEditMode = true)]
        public DateTime? CfpStart { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
               ApplyFormatInEditMode = true)]
        public DateTime? CfpEnd { get; set; }

        [StringLength(50)]
        [Display(Name = "VIP invite code")]
        public string CfpVipCode { get; set; }

        [Display(Name = "Min speakers on front page")]
        public int MinNumberOfSpeakers { get; set; }

        public ICollection<Speaker> Speakers { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Sponsor> Sponsors { get; set; }
        public ICollection<Slot> Slots { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<ImageType> ImageTypes { get; set; }
        public ICollection<SessionType> SessionTypes { get; set; }

    }
}