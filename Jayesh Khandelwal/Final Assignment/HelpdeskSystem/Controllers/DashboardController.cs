using HelpdeskSystem.Models;
using HelpdeskSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HelpdeskSystem.Controllers
{
    [Authorize(Roles = "CompanyAdmin")]
    public class DashboardController : Controller
    {
        private readonly ITicketService _ticketService;
        public DashboardController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = await _ticketService.DashboardDataAsync();
            return View(dashboard);
        }

        [HttpGet("/api/dashboard")]
        [Authorize(Roles = "CompanyAdmin")]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _ticketService.DashboardDataAsync();

            return Ok(dashboard);
        }
    }
}