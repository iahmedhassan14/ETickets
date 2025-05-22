using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETickets.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Display(Name = "Movie Title")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 1000 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000.00, ErrorMessage = "Price must be between $0.01 and $1000.00")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public string? ImgUrl { get; set; }

        [Required(ErrorMessage = "Trailer URL is required")]
        [Display(Name = "Trailer URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string TrailerUrl { get; set; } = null!;

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DateGreaterThan("StartDate", ErrorMessage = "End date must be after start date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        [StringLength(20)]
        public string MovieStatus { get; set; } = null!;  // Consider using enum instead

        [Required(ErrorMessage = "Rating is required")]
        [Display(Name = "Movie Rating")]
        [StringLength(10)]
        public string MovieRate { get; set; } = null!;  // Consider using enum instead

        [Required(ErrorMessage = "PG rating is required")]
        [Display(Name = "PG Rating")]
        [StringLength(10)]
        public string PgRate { get; set; } = null!;  // Consider using enum instead

        [Display(Name = "Tags")]
        [StringLength(200)]
        public string Tags { get; set; } = null!;

        // Relationships
        [Display(Name = "Available Cinemas")]
        public ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();

        [Required(ErrorMessage = "At least one category is required")]
        [Display(Name = "Categories")]
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        [Display(Name = "Cast")]
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
    }

    // Custom validation attribute for date comparison
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue < comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}