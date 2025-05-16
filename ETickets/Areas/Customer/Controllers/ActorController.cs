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
                .Include(e => e.Movies)
                    .ThenInclude(x => x.Categories)
                .FirstOrDefault(e => e.Id == id);
            
            if (actor == null)
            {
                return RedirectToAction("Index" , "Home");
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
