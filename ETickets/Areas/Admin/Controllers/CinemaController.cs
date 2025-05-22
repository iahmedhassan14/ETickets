using ETickets.Data;
using ETickets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        private const string ImagePath = "assets/img/cinemas/";

        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas.ToListAsync();
            return View(cinemas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile CinemaLogo)
        {
            ModelState.Remove("CinemaLogo");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to create Cinema|Create Failed";
                return View(cinema);
            }

            if (CinemaLogo == null || CinemaLogo.Length == 0)
            {
                TempData["Error"] = "Profile picture is required|Image Required";
                return View(cinema);
            }
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(CinemaLogo.FileName).ToLower();

            if (!validExtensions.Contains(extension))
            {
                TempData["Error"] = "Only JPG, JPEG, PNG, and GIF images are allowed|Invalid Image";
                return View(cinema);
            }

            if (CinemaLogo.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "Image must be smaller than 5MB|Image Too Large";
                return View(cinema);
            }

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath.TrimStart('/'));
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await CinemaLogo.CopyToAsync(fileStream);
                }

                cinema.CinemaLogo = uniqueFileName;

                await _context.Cinemas.AddAsync(cinema);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cinema created successfully|Cinema Created";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating Cinema: {ex.Message}|Create Failed";
                return View(cinema);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                TempData["Error"] = "Cinema not found|Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema, IFormFile CinemaLogo)
        {
            if (id != cinema.Id)
            {
                TempData["Error"] = "Invalid cinema ID|Edit Failed";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("CinemaLogo");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form|Edit Failed";
                return View(cinema);
            }

            try
            {
                var existingCinema = await _context.Cinemas.FindAsync(id);
                if (existingCinema == null)
                {
                    TempData["Error"] = "Cinema not found|Not Found";
                    return RedirectToAction(nameof(Index));
                }

                if (CinemaLogo != null && CinemaLogo.Length > 0)
                {

                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(CinemaLogo.FileName).ToLower();

                    if (!validExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Only JPG, JPEG, PNG, and GIF images are allowed|Invalid Image";
                        return View(cinema);
                    }

                    if (CinemaLogo.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "Image must be smaller than 5MB|Image Too Large";
                        return View(cinema);
                    }

                    if (!string.IsNullOrEmpty(existingCinema.CinemaLogo))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, existingCinema.CinemaLogo.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }


                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath.TrimStart('/'));
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await CinemaLogo.CopyToAsync(fileStream);
                    }

                    existingCinema.CinemaLogo = uniqueFileName;
                }


                existingCinema.Name = cinema.Name;
                existingCinema.Description = cinema.Description;
                existingCinema.Address = cinema.Address;

                _context.Cinemas.Update(existingCinema);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cinema updated successfully|Cinema Updated";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating cinema: {ex.Message}|Edit Failed";
                return View(cinema);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null)
            {
                TempData["Error"] = "Cinema not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {

                if (!string.IsNullOrEmpty(cinema.CinemaLogo))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath , cinema.CinemaLogo.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cinema deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete cinema: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}