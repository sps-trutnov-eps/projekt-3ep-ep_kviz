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
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "Neplatné údaje.";
                return View();
            }
            var newuser = new UzivateleModel
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newuser, model.Password);

            if (!result.Succeeded)
            {
                ViewBag.Message = "Registrace se nezdařila: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return View();
            }

            ViewBag.Message = "Registrace úspěšná!";
            return View();
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
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Message = "Neplatné údaje.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.Message = "Přihlášení se nezdařilo. Zkontrolujte uživatelské jméno a heslo.";
                return View();
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
