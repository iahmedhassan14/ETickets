using ETickets.Data;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(int id)
        {
            var actor = _context.Actors
                .Include(a => a.Movies)
                    .ThenInclude(m => m.Categories)
                .FirstOrDefault(a => a.Id == id);
            
            if (actor == null)
            {
                return NotFound();
            }

            ActorsWithMoviesVM ActorsWithMoviesVM = new()
            {
                Actors = actor,
                Movies = actor.Movies.ToList()
            };
            return View(ActorsWithMoviesVM);
        }
    }
}
