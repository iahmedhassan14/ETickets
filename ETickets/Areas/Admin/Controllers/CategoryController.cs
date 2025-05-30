﻿using ETickets.Data;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var category = _context.Categories;
            return View(category.ToList());
        }
        public IActionResult Create()
        { 
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category) 
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id) 
        {
           var category =  _context.Categories.Find(id);
            if (category is not null)
            {
                return View(category);
            }
            return RedirectToAction("NotFoundPage", "Home" );
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category is not null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
