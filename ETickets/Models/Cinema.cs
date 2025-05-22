using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETickets.Models
{
    public class Cinema
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Cinema Name")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Logo URL is required")]
        [Display(Name = "Logo URL")]
        [Url(ErrorMessage = "Please enter a valid URL for the logo")]
        public string CinemaLogo { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Address must be between 10 and 200 characters")]
        [Display(Name = "Full Address")]
        public string Address { get; set; } = null!;

        [Display(Name = "Movies")]
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}