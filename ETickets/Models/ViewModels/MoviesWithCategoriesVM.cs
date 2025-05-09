namespace ETickets.Models.ViewModels
{
    public class MoviesWithCategoriesVM
    {
        public List<Movie> Movies { get; set; } = null!;
        public List<Category> Categories { get; set; } = null!;
        public double TotalPageNumber { get; set; }
    }
}
