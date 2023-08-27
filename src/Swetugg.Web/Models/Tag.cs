using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swetugg.Web.Models
{
    public class SessionType
    {
        public int Id { get; set; }
        public int ConferenceId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        public int Priority { get; set; }

        public ICollection<Session> Sessions { get; set; }

    }

    public enum TagType
    {
        Session,
        Speaker
    }

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

        [DataType(DataType.MultilineText)]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Featured { get; set; }
        public int Priority { get; set; }

        [Required]
        [DefaultValue(TagType.Session)]
        public TagType Type { get; set; }

        public ICollection<Session> Sessions { get; set; }
        public ICollection<Speaker> Speakers { get; set; }
    }
}