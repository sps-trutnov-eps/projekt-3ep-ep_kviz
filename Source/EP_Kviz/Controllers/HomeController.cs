using System.Diagnostics;
using EP_Kviz.Models;
using Microsoft.AspNetCore.Mvc;

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
            

        private static List<User> users = new();
        private static int nextId = 1;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (users.Any(u => u.Username == model.Username))
                {
                    ViewBag.Message = "Uživatel již existuje.";
                    return View();
                }

                var user = new User
                {
                    Id = nextId++,
                    Username = model.Username,
                    Password = model.Password
                };
                users.Add(user);

                ViewBag.RegistrationSuccess = true;
                ModelState.Clear(); // Vyèistí formuláø
                return View();
            }
            
            ViewBag.RegistrationSuccess = false;
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user != null)
            {
                // Uložení ID do session
                HttpContext.Session.SetInt32("UserId", user.Id);

                // Pøesmìrování na výbìr hry
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Špatné jméno nebo heslo.";
                return View();
            }
        }


    }
}
