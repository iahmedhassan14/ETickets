using ETickets.Data;
using ETickets.Models;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
             var movie = _context.Movies
                .Include(e => e.Categories)
                .Include(e => e.Cinemas)
                .Select(e => new MoviesWithCategoriesWithCinemasVM
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Price = e.Price,
                    Categories = e.Categories != null ? string.Join(", ", e.Categories.Select(x => x.Name)) : "",
                    Cinemas = e.Cinemas != null ? string.Join(", ", e.Cinemas.Select(x => x.Name)) : ""
                })
                .ToList();

             return View(movie);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Actors = _context.Actors.Select(a => new {
                Id = a.Id,
                FullName = a.FirstName + " " + a.LastName
            }).ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie, List<int> Categories, List<int> Actors, List<int> Cinemas)
        {
            movie.Categories = _context.Categories.Where(c => Categories.Contains(c.Id)).ToList();
            movie.Actors = _context.Actors.Where(a => Actors.Contains(a.Id)).ToList();
            movie.Cinemas = _context.Cinemas.Where(c => Cinemas.Contains(c.Id)).ToList();

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Categories)
                .Include(m => m.Actors)
                .Include(m => m.Cinemas)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Actors = _context.Actors.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, List<int> categoryIds, List<int> actorIds, List<int> cinemaIds)
        {
            var existingMovie = _context.Movies
                .Include(m => m.Categories)
                .Include(m => m.Actors)
                .Include(m => m.Cinemas)
                .FirstOrDefault(m => m.Id == movie.Id);

            if (existingMovie == null) return NotFound();

            existingMovie.Name = movie.Name;
            existingMovie.Description = movie.Description;
            existingMovie.Price = movie.Price;
            existingMovie.ImgUrl = movie.ImgUrl;
            existingMovie.StartDate = movie.StartDate;
            existingMovie.EndDate = movie.EndDate;
            existingMovie.TrailerUrl = movie.TrailerUrl;
            existingMovie.MovieStatus = movie.MovieStatus;
            existingMovie.MovieRate = movie.MovieRate;
            existingMovie.PgRate = movie.PgRate;
            existingMovie.Tags = movie.Tags;

            existingMovie.Categories = _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToList();
            existingMovie.Actors = _context.Actors.Where(a => actorIds.Contains(a.Id)).ToList();
            existingMovie.Cinemas = _context.Cinemas.Where(c => cinemaIds.Contains(c.Id)).ToList();

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie is not null)
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
