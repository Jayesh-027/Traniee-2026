using HelpdeskSystem.Models;
using HelpdeskSystem.Services;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound();
            }
            if (User.IsInRole("CompanyAdmin") || User.IsInRole("Agent"))
            {
                ViewBag.Agents = await _ticketService.GetAgentsAsync();
            }
            return View(result.Value);  
        }

        [HttpGet("/tickets/{id}/Assign")]
        [Authorize(Roles = "CompanyAdmin,Agent")]
        public async Task<IActionResult> Assign(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);
            if (result.IsFailure)
            {
                return NotFound();
            }
            ViewBag.Agents = await _ticketService.GetAgentsAsync();
            return View(result.Value);
        }

        [HttpPost("/api/tickets/{id}/assign")]
        [Authorize(Roles = "CompanyAdmin,Agent")]
     
        public async Task<IActionResult> Assign(int id, string agentId)
        {
            var result = await _ticketService.AssignTicketAsync(id, agentId);
            if (result.IsFailure)
            {
                return BadRequest(result.ErrorMessage);
            }
            return RedirectToAction("Details", new { id });
        }
        [HttpGet("/tickets/{id}/Status")]
        [Authorize(Roles = "CompanyAdmin,Agent")]
        public async Task<IActionResult> Status(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);

            if (result.IsFailure)
            {
                return NotFound();
            }

            return View(result.Value);
        }

    }
}
