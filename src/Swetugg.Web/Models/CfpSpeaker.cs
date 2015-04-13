using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class CfpSpeaker
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        public string UserId { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Full name")]
        public string Name { get; set; }

        [StringLength(250)]
        public string Company { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Bio { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [StringLength(250)]
        [Display(Name = "Web site")]
        public string Web { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [StringLength(50)]
        [Display(Name = "Twitter")]
        public string Twitter { get; set; }

        [StringLength(250)]
        [Display(Name = "GitHub user")]
        public string GitHub { get; set; }

        public ICollection<CfpSession> Sessions { get; set; }
        
        public Speaker Speaker { get; set; }
        public int? SpeakerId { get; set; }
    }
}