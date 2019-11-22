using System.ComponentModel.DataAnnotations;

namespace Swetugg.Web.Models
{
    public class SponsorImage
    {
        public int Id { get; set; }

        public int SponsorId { get; set; }

        [Required]
        public int ImageTypeId { get; set; }

        [Required]
        public ImageType ImageType { get; set; }

        [Required]
        [StringLength(1000)]
        public string ImageUrl { get; set; }
    }
}