using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hmmm... You should really add a Name...")]
        [MaxLength(35)]
        public string? Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The breed cannot exceed 50 characters.")]
        public string? Breed { get; set; }

        [StringLength(500, ErrorMessage = "The notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }

        [Url(ErrorMessage = "The image URL is not valid.")]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        public int OwnerId { get; set; } //C# code sets it
        // ASP.NET gets current user ID, and C# auto sets OwnerId to current user's ID
        

    }
}
