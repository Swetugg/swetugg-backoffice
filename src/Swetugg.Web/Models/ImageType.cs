using System.ComponentModel.DataAnnotations;

namespace Swetugg.Web.Models
{
    public class ImageType
    {
        public int Id { get; set; }
        
        public int ConferenceId { get; set; }
        
        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Slug { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }

    }
}