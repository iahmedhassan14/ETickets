using ETickets.Data;
using ETickets.Models;
using ETickets.Models.ViewModels;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        private const string ImagePath = "assets/img/movies/";

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies
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
                .ToListAsync();

            return View(movies);
        }

        public IActionResult Create()
        {
            MovieVM movieVM = new()
            {
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                Actors = _context.Actors.Select(e => new ActorVM
                {
                    Id = e.Id,
                    FullName = e.FirstName + " " + e.LastName
                }).ToList()
            };

            return View(movieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieVM movieVM, IFormFile ImgUrl)
        {
            if (!ModelState.IsValid)
            {
                movieVM.Categories = _context.Categories.ToList();
                movieVM.Cinemas = _context.Cinemas.ToList();
                movieVM.Actors = _context.Actors
                    .Select(e => new ActorVM
                    {
                        Id = e.Id,
                        FullName = e.FirstName + " " + e.LastName
                    }).ToList();

                TempData["Error"] = "Failed to create movie. Please check the form.";
                return View(movieVM);
            }

            string? imageName = null;
            if (ImgUrl != null && ImgUrl.Length > 0)
            {
                imageName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, imageName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ImgUrl.CopyToAsync(stream);
                }
            }

            Movie movie = new()
            {
                Name = movieVM.Name,
                Description = movieVM.Description,
                Price = movieVM.Price,
                ImgUrl = imageName,
                TrailerUrl = movieVM.TrailerUrl,
                StartDate = movieVM.StartDate,
                EndDate = movieVM.EndDate,
                MovieStatus = movieVM.MovieStatus,
                MovieRate = movieVM.MovieRate,
                PgRate = movieVM.PgRate,
                Tags = movieVM.Tags
            };

            movie.Categories = await _context.Categories
                .Where(c => movieVM.CategoryIds.Contains(c.Id)).ToListAsync();

            movie.Cinemas = await _context.Cinemas
                .Where(c => movieVM.CinemaIds.Contains(c.Id)).ToListAsync();

            movie.Actors = await _context.Actors
                .Where(a => movieVM.ActorIds.Contains(a.Id)).ToListAsync();

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Categories)
                .Include(m => m.Cinemas)
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }

            var movieVM = new MovieVM
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                ImgUrl = movie.ImgUrl,
                TrailerUrl = movie.TrailerUrl,
                StartDate = movie.StartDate,
                EndDate = movie.EndDate,
                MovieStatus = movie.MovieStatus,
                MovieRate = movie.MovieRate,
                PgRate = movie.PgRate,
                Tags = movie.Tags,
                CategoryIds = movie.Categories.Select(c => c.Id).ToList(),
                CinemaIds = movie.Cinemas.Select(c => c.Id).ToList(),
                ActorIds = movie.Actors.Select(a => a.Id).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Cinemas = await _context.Cinemas.ToListAsync(),
                Actors = await _context.Actors
                    .Select(a => new ActorVM
                    {
                        Id = a.Id,
                        FullName = a.FirstName + " " + a.LastName
                    }).ToListAsync()
            };

            return View(movieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieVM movieVM, IFormFile ImgUrl)
        {
            if (id != movieVM.Id)
                return NotFound();

            ModelState.Remove("ImgUrl");

            if (!ModelState.IsValid)
            {
                movieVM.Categories = await _context.Categories.ToListAsync();
                movieVM.Cinemas = await _context.Cinemas.ToListAsync();
                movieVM.Actors = await _context.Actors
                    .Select(a => new ActorVM
                    {
                        Id = a.Id,
                        FullName = a.FirstName + " " + a.LastName
                    }).ToListAsync();

                TempData["Error"] = "Failed to update movie. Please check the form.";
                return View(movieVM);
            }

            var movie = await _context.Movies
                .Include(m => m.Categories)
                .Include(m => m.Cinemas)
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }

            if (ImgUrl != null && ImgUrl.Length > 0)
            {
                if (!string.IsNullOrEmpty(movie.ImgUrl))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, movie.ImgUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                string imageName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, imageName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await ImgUrl.CopyToAsync(stream);
                }

                movie.ImgUrl = imageName;
            }
            else
            {
                movieVM.ImgUrl = movie.ImgUrl;
            }
            movie.Name = movieVM.Name;
            movie.Description = movieVM.Description;
            movie.Price = movieVM.Price;
            movie.TrailerUrl = movieVM.TrailerUrl;
            movie.StartDate = movieVM.StartDate;
            movie.EndDate = movieVM.EndDate;
            movie.MovieStatus = movieVM.MovieStatus;
            movie.MovieRate = movieVM.MovieRate;
            movie.PgRate = movieVM.PgRate;
            movie.Tags = movieVM.Tags;

            movie.Categories = await _context.Categories
                .Where(c => movieVM.CategoryIds.Contains(c.Id)).ToListAsync();

            movie.Cinemas = await _context.Cinemas
                .Where(c => movieVM.CinemaIds.Contains(c.Id)).ToListAsync();

            movie.Actors = await _context.Actors
                .Where(a => movieVM.ActorIds.Contains(a.Id)).ToListAsync();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Movie created successfully|Operation Successful";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (!string.IsNullOrEmpty(movie.ImgUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, movie.ImgUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Movie deleted successfully!";
            }
            catch
            {
                TempData["Error"] = "Failed to delete movie.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
