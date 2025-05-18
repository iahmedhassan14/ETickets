using ETickets.Data;
using ETickets.Models;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var movies = _context.Movies
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

            return View(movies);
        }

        public IActionResult Create()
        {
            LoadViewData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, List<int> Categories, List<int> Actors, List<int> Cinemas, IFormFile ImgUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadViewData();
                    return View(movie);
                }

                // Handle image upload
                if (ImgUrl != null && ImgUrl.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "Movies");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(ImgUrl.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImgUrl.CopyToAsync(stream);
                    }

                    movie.ImgUrl = uniqueFileName;
                }
                else
                {
                    TempData["Error"] = "Please upload an image";
                    LoadViewData();
                    return View(movie);
                }

                // Set relationships
                movie.Categories = await _context.Categories.Where(c => Categories.Contains(c.Id)).ToListAsync();
                movie.Actors = await _context.Actors.Where(a => Actors.Contains(a.Id)).ToListAsync();
                movie.Cinemas = await _context.Cinemas.Where(c => Cinemas.Contains(c.Id)).ToListAsync();

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Movie created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating movie: {ex.Message}";
                LoadViewData();
                return View(movie);
            }
        }

        public IActionResult Edit(int id)
        {
            var movie = GetMovieWithRelationships(id);
            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Actors = _context.Actors
                .Select(a => new {
                    Id = a.Id,
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Movie movie, List<int> Categories, List<int> Actors, List<int> Cinemas, IFormFile ImgUrl, string removeImageFlag)
        {
            try
            {
                var existingMovie = await _context.Movies
                    .Include(m => m.Categories)
                    .Include(m => m.Actors)
                    .Include(m => m.Cinemas)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (existingMovie == null)
                {
                    return Json(new { success = false, message = "Movie not found" });
                }

                // Handle image removal
                if (removeImageFlag == "true")
                {
                    ApplicationDbContext.DeleteImage(existingMovie.ImgUrl);
                    existingMovie.ImgUrl = null;
                }

                // Handle new image upload
                if (ImgUrl != null && ImgUrl.Length > 0)
                {
                    ApplicationDbContext.DeleteImage(existingMovie.ImgUrl); // Delete old image
                    existingMovie.ImgUrl = ApplicationDbContext.SaveImage(ImgUrl);
                }

                // Update other properties and relationships
                existingMovie.Name = movie.Name;
                existingMovie.Description = movie.Description;
                // ... other properties ...

                existingMovie.Categories = await _context.Categories
                    .Where(c => Categories.Contains(c.Id))
                    .ToListAsync();
                existingMovie.Actors = await _context.Actors
                    .Where(a => Actors.Contains(a.Id))
                    .ToListAsync();
                existingMovie.Cinemas = await _context.Cinemas
                    .Where(c => Cinemas.Contains(c.Id))
                    .ToListAsync();

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
            {
                TempData["Error"] = "Movie not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Movies.Remove(movie);
                _context.SaveChanges();
                TempData["Success"] = "Movie deleted successfully!";
            }
            catch
            {
                TempData["Error"] = "Failed to delete movie.";
            }

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods
        private void LoadViewData()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Actors = _context.Actors.Select(a => new {
                Id = a.Id,
                FullName = $"{a.FirstName} {a.LastName}"
            }).ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
        }

        private Movie? GetMovieWithRelationships(int id)
        {
            try
            {
                return _context.Movies
                    .AsNoTracking()
                    .Include(m => m.Categories)
                    .Include(m => m.Actors)
                    .Include(m => m.Cinemas)
                    .FirstOrDefault(m => m.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void SetMovieRelationships(Movie movie, List<int> categories, List<int> actors, List<int> cinemas)
        {
            movie.Categories = _context.Categories.Where(c => categories.Contains(c.Id)).ToList();
            movie.Actors = _context.Actors.Where(a => actors.Contains(a.Id)).ToList();
            movie.Cinemas = _context.Cinemas.Where(c => cinemas.Contains(c.Id)).ToList();
        }

        private void UpdateMovieProperties(Movie existing, Movie updated)
        {
            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.Price = updated.Price;
            existing.StartDate = updated.StartDate;
            existing.EndDate = updated.EndDate;
            existing.TrailerUrl = updated.TrailerUrl;
            existing.MovieStatus = updated.MovieStatus;
            existing.MovieRate = updated.MovieRate;
            existing.PgRate = updated.PgRate;
            existing.Tags = updated.Tags;
        }

        private void HandleImageUpload(Movie movie, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0) return;
            const string relativeUploadPath = "/assets/img/Movies/";
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativeUploadPath.TrimStart('/'));
            Directory.CreateDirectory(fullPath);

            using (var stream = new FileStream(Path.Combine(fullPath, uniqueFileName), FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }
            movie.ImgUrl = Path.Combine(relativeUploadPath, uniqueFileName).Replace("\\", "/");
        }

        private void HandleImageRemoval(Movie movie, string imageToRemove)
        {
            if (string.IsNullOrEmpty(imageToRemove)) return;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "Movies", imageToRemove);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            movie.ImgUrl = null;
        }
        #endregion
    }
}