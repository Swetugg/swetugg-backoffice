using System.ComponentModel.DataAnnotations;

namespace Swetugg.Web.Models
{
    public class SpeakerImage
    {
        public int Id { get; set; }

        public int SpeakerId { get; set; }

        [Required]
        public int ImageTypeId { get; set; }

        [Required]
        public ImageType ImageType { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string ImageUrl { get; set; }
    }
}