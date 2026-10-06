using HelpdeskSystem.Services;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class UserController : Controller
    {
        
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
           
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUser()
        {
            var agents = await _userService.GetAgentsAsync();
            var model = new ManageUsersViewModel
            {
                Agents = agents
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAgentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.CreateAgentAsync(model);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            return RedirectToAction(nameof(ManageUser));
        }
    }
}
