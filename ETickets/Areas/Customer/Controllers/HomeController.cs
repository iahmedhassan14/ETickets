using System.Diagnostics;
using ETickets.Data;
using ETickets.Models;
using ETickets.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context = new();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IQueryable<Movie> movies = _context.Movies.Include(e => e.Categories);
            var categories = _context.Categories;

            MoviesWithCategoriesVM MoviesWithCategoriesVM = new()
            {
                Movies = _context.Movies.Include(e => e.Categories).ToList()
            };
            return View(MoviesWithCategoriesVM);
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
