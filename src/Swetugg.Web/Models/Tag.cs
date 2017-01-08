using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Swetugg.Web.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Featured { get; set; }

        public ICollection<Session> Sessions { get; set; }
    }
}