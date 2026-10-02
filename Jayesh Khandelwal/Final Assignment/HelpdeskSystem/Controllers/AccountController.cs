using HelpdeskSystem.Repositories;
using HelpdeskSystem.Services;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ICompanyRepository _companyRepository;

        public AccountController(IAuthService authService, ICompanyRepository companyrepository)
        {
            _authService = authService;
            _companyRepository = companyrepository;
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var companies = await _companyRepository.GetAllAsync();
            ViewBag.Companies = companies;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _authService.RegisterAsync(model);   
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _authService.LoginAsync(model);
            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }
            return RedirectToAction("Index", "Ticket");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}
