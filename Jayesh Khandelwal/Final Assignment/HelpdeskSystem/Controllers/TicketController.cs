using HelpdeskSystem.Services;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace HelpdeskSystem.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? status, string? priority, string? assignee, bool overdue = false,string? search= null, string? sort = null, int page = 1, int pageSize =10)
        {
            var tickets = await _ticketService.GetTicketsAsync(status,priority,assignee,overdue,search, sort,page, pageSize);
            if (User.IsInRole("CompanyAdmin") || User.IsInRole("Agent"))
            {
                ViewBag.Agents = await _ticketService.GetAgentsAsync();
            }
            ViewBag.Status = status;
            ViewBag.Priority = priority;
            ViewBag.Assignee = assignee;
            ViewBag.Overdue = overdue;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.Page = page;

            return View(tickets);
        }

        [HttpGet]
        public IActionResult CreateTicket()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTicket(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _ticketService.CreateTicketAsync(model);
            if (!result)
            {
                ModelState.AddModelError("", "Unable to create ticket.");
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}
