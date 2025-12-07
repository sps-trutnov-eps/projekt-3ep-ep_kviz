using System.Diagnostics;
using EP_Kviz.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace EP_Kviz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
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

        // New action: Tabulka skóre - top5 podle PocetVyhranychHer
        public IActionResult TabulkaSkore()
        {
            var top = _context.Users
                .OrderByDescending(u => u.PocetVyhranychHer)
                .ThenBy(u => u.UserName)
                .Take(5)
                .ToList();

            return View(top);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
