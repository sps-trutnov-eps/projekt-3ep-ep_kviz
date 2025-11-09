using EP_Kviz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EP_Kviz.Controllers
{
    public class UzivateleController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UzivateleModel> _userManager;
        private readonly SignInManager<UzivateleModel> _signInManager;
        public UzivateleController(AppDbContext context, SignInManager<UzivateleModel> signInManager, UserManager<UzivateleModel> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
            {
                ViewBag.Message = "Neplatné údaje.";
                return View();
            }



            //Kontrola jestli username existuje
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                ViewBag.Message = "Uživatelské jméno již existuje.";
                return View(model);
            }




            var newuser = new UzivateleModel
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newuser, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.Message = "Registrace se nezdařila";
                return View();
            }

            return RedirectToAction("Login", "Uzivatele");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(model.Username) && string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "Zadejte přihlašovací údaje.";
                return View();
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "Neplatné údaje. " +
                    "Zadejte heslo";
                return View();
            }
            if (string.IsNullOrEmpty(model.Username))
            {
                ViewBag.Message = "Neplatné údaje. " +
                    "Zadejte uživatelské jméno";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.Message = "Přihlášení se nezdařilo. Zkontrolujte uživatelské jméno a heslo.";
                return View();
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
