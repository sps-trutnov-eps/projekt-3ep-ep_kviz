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
            if (userId == null)
            {

                ViewBag.Message = "U�ivatel nen� p�ihl�en";
            }
            else {
                ViewBag.Message = "U�ivatel je p�ihl�en�";
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

        


        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (users.Any(u => u.Username == model.Username))
                {
                    ViewBag.Message = "U�ivatel ji� existuje.";
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
                ModelState.Clear(); // Vy�ist� formul��
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
                // Ulo�en� ID do session
                HttpContext.Session.SetInt32("UserId", user.Id);

                // P�esm�rov�n� na v�b�r hry
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "�patn� jm�no nebo heslo.";
                return View();
            }
        }


    }
}
