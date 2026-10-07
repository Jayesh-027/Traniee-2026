using HelpdeskSystem.Models;
using HelpdeskSystem.Services;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace HelpdeskSystem.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketWorkflowService _ticketWorkflowService;

        public TicketController(ITicketService ticketService, ITicketWorkflowService ticketWorkflowService)
        {
            _ticketService = ticketService;
            _ticketWorkflowService = ticketWorkflowService;
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

        [HttpPost("/tickets/{id}/delete")]
        [Authorize(Roles = "CompanyAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteTicketAsync(id);

            if (result.IsFailure)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
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
     
        public async Task<IActionResult> UpdateAssign(int id, string agentId)
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

            ViewBag.NextStatuses =
                _ticketWorkflowService.GetNextStatuses(
                    result.Value.Status);

            return View(result.Value);
        }

        [HttpPost("/api/tickets/{id}/status")]
        [Authorize(Roles = "CompanyAdmin,Agent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id,TicketStatus status, byte[] rowVersion)
        {
            var result = await _ticketService.ChangeStatusAsync(id,status,rowVersion);
            if (result.IsFailure)
            {
                return BadRequest(result.ErrorMessage);
            }

            return RedirectToAction("Details", new { id });
        }

        [HttpGet("/tickets/{id}/Comments")]
        [Authorize]
        public async Task<IActionResult> Comments(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);

            if (result.IsFailure)
                return NotFound();

            var comments = await _ticketService.GetCommentsAsync(id);

            ViewBag.Comments = comments;

            return View(result.Value);
        }

        

        [HttpPost("/api/tickets/{id}/comments")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            var result = await _ticketService.AddCommentAsync(id, comment);

            if (result.IsFailure)
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.ErrorMessage
                });
            }

            return Ok(new
            {
                success = true,
                message = "Comment added successfully."
            });
        }

        [HttpGet("/tickets/{id}/History")]
        [Authorize(Roles = "CompanyAdmin,Agent")]
        public async Task<IActionResult> History(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);

            if (result.IsFailure) { 
                return NotFound();}

            var history = await _ticketService.GetHistoryAsync(id);

            ViewBag.History = history;

            return View(result.Value);
        }
        [HttpGet("/tickets/{id}/Edit")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);

            if (result.IsFailure)
            {
                return NotFound();
            }

            var ticket = result.Value;

            if (ticket.AssignedToUserId != null ||
                ticket.Status == TicketStatus.Closed)
            {
                return BadRequest("This ticket cannot be edited.");
            }

            var model = new EditTicketViewModel
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                RowVersion = ticket.RowVersion
            };

            return View(model);
        }

        [HttpPost("/tickets/{id}/Edit")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,EditTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _ticketService.EditTicketAsync(id, model);

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
