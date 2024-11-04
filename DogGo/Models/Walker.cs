using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hmmm... You should really add a Name...")]
        [MaxLength(35)]
        public string? Name { get; set; }

        [Required]
        [DisplayName("Neighborhood")]
        public int NeighborhoodId { get; set; }

        [Url(ErrorMessage = "The image URL is not valid.")]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        public Neighborhood Neighborhood { get; set; }
        
    }
}