using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
    public class CinemaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
