using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
