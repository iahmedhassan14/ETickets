using ETickets.Data;
using ETickets.Models;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int id)
        {
            var movie = _context.Movies
                            .Include(m => m.Categories)
                            .Include(m => m.Actors)
                            .Include(m => m.Cinemas)
                            .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MoviesWithCategoriesWithActorsWithCinemasVM
            {
                Movies = new List<Movie> { movie },
                Categories = _context.Categories.ToList(),
                Actors = _context.Actors.ToList(),
                Cinemas = _context.Cinemas.ToList()
            };


            return View(viewModel);
        }
    
    }
}
