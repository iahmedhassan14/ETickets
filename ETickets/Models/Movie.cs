namespace ETickets.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImgUrl { get; set; }
        public string TrailerUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string MovieStatus { get; set; }
        public string MovieRate { get; set; }
        public string PgRate { get; set; }
        public string Tags { get; set; }


        public ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
        
    }
}
