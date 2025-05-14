namespace ETickets.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; } 
        public string ImgUrl { get; set; } = null!;
        public string TrailerUrl { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MovieStatus { get; set; } = null!;
        public string MovieRate { get; set; } = null!;
        public string PgRate { get; set; } = null!;
        public string Tags { get; set; } = null!;


        public ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
        
    }
}
