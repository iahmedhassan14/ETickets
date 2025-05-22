using ETickets.Data;
using ETickets.Models;
using ETickets.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        private const string ImagePath = "assets/img/cast/";

        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors.ToListAsync();
            return View(actors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor, IFormFile profilePicture)
        {
            ModelState.Remove("ProfilePicture");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to create actor|Create Failed";
                return View(actor);
            }

            if (profilePicture == null || profilePicture.Length == 0)
            {
                TempData["Error"] = "Profile picture is required|Image Required";
                return View(actor);
            }
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(profilePicture.FileName).ToLower();

            if (!validExtensions.Contains(extension))
            {
                TempData["Error"] = "Only JPG, JPEG, PNG, and GIF images are allowed|Invalid Image";
                return View(actor);
            }

            if (profilePicture.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "Image must be smaller than 5MB|Image Too Large";
                return View(actor);
            }

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath.TrimStart('/'));
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(fileStream);
                }

                actor.ProfilePicture = uniqueFileName;

                await _context.Actors.AddAsync(actor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Actor created successfully|Actor Created";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating actor: {ex.Message}|Create Failed";
                return View(actor);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                TempData["Error"] = "Actor not found|Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile profilePicture)
        {
            if (id != actor.Id)
            {
                TempData["Error"] = "Invalid actor ID|Edit Failed";
                return RedirectToAction(nameof(Index));
            }

            ModelState.Remove("ProfilePicture");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors in the form|Edit Failed";
                return View(actor);
            }

            try
            {
                var existingActor = await _context.Actors.FindAsync(id);
                if (existingActor == null)
                {
                    TempData["Error"] = "Actor not found|Not Found";
                    return RedirectToAction(nameof(Index));
                }

                if (profilePicture != null && profilePicture.Length > 0)
                {

                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(profilePicture.FileName).ToLower();

                    if (!validExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Only JPG, JPEG, PNG, and GIF images are allowed|Invalid Image";
                        return View(actor);
                    }

                    if (profilePicture.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "Image must be smaller than 5MB|Image Too Large";
                        return View(actor);
                    }

                    if (!string.IsNullOrEmpty(existingActor.ProfilePicture))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, existingActor.ProfilePicture.TrimStart('/'));
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
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    existingActor.ProfilePicture = uniqueFileName;
                }


                existingActor.FirstName = actor.FirstName;
                existingActor.LastName = actor.LastName;
                existingActor.Bio = actor.Bio;
                existingActor.News = actor.News;

                _context.Actors.Update(existingActor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Actor updated successfully|Actor Updated";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating actor: {ex.Message}|Edit Failed";
                return View(actor);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                TempData["Error"] = "Actor not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {

                if (!string.IsNullOrEmpty(actor.ProfilePicture))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath, actor.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Actor deleted successfully!";
            }
            catch
            {
                TempData["Error"] = "Failed to delete actor.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}