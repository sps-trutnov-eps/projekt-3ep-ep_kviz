using System.Diagnostics;
using EP_Kviz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EP_Kviz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (userId == null)
            {

                ViewBag.Message = "Žádný uživatel není přihlášen";
            }
            else {
                ViewBag.Message = "Je přihlášen uživatel: " + userName;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Pravidla()
        {
            return View();
        }

        public IActionResult Vyber()
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
