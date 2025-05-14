namespace ETickets.Models.ViewModels
{
    public class MoviesWithCategoriesWithActorsWithCinemasVM
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
        public IEnumerable<Cinema> Cinemas { get; set; } = new List<Cinema>();
    }
}
