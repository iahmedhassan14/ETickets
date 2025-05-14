namespace ETickets.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CinemaLogo { get; set; } = null!;
        public string Address { get; set; } = null!;

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
