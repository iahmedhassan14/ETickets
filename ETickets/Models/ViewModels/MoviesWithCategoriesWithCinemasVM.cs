namespace ETickets.Models.ViewModels
{
    public class MoviesWithCategoriesWithCinemasVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Categories { get; set; } = string.Empty;
        public string Cinemas { get; set; } = string.Empty;
    }
}
