using HelpdeskSystem.Helpers;
using HelpdeskSystem.Models;
using HelpdeskSystem.Repositories;
using HelpdeskSystem.ViewModels;

namespace HelpdeskSystem.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ITicketWorkflowService _ticketWorkflowService;

        public TicketService(ITicketRepository ticketRepository,ICurrentUserService currentUserService,IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ITicketWorkflowService ticketWorkflowService)
        {
            _ticketRepository = ticketRepository;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _ticketWorkflowService = ticketWorkflowService;
        }

        public async Task<List<Ticket>> GetTicketsAsync(string? status,string? priority,string? assignee,bool overdue,string? search,string? sort,int page,int pageSize)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (user.IsInRole("Customer"))
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                return await _ticketRepository.GetByUserIdAsync(userId);
            }
            return await _ticketRepository.GetAllAsync(status,priority,assignee, overdue,search,sort,page,pageSize);
        }
        public async Task<List<ApplicationUser>> GetAgentsAsync()
        {
            var companyId = _currentUserService.GetCompanyId();
            return await _userRepository.GetAgentsByCompanyIdAsync(companyId);
        }

        public async Task<bool> CreateTicketAsync(CreateTicketViewModel model)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                .Value;
            if (userId == null)
            {
                return false;
            }
            var companyId = _currentUserService.GetCompanyId();
            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority,
                Status = TicketStatus.New,
                CompanyId = companyId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.Now
            };
            if (model.Priority == TicketPriority.High)
            {
                ticket.FirstResponseDue = DateTime.Now.AddHours(1);
                ticket.ResolutionDue = DateTime.Now.AddHours(8);
            }
            else if (model.Priority == TicketPriority.Medium)
            {
                ticket.FirstResponseDue = DateTime.Now.AddHours(4);
                ticket.ResolutionDue = DateTime.Now.AddHours(24);
            }
            else
            {
                ticket.FirstResponseDue = DateTime.Now.AddHours(12);
                ticket.ResolutionDue = DateTime.Now.AddHours(72);
            }

            return await _ticketRepository.CreateAsync(ticket);
        }
        public async Task<Result<Ticket>> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return Result<Ticket>.Failure("Ticket not found.");
            }
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null && user.IsInRole("Customer"))
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (ticket.CreatedByUserId != userId)
                {
                    return Result<Ticket>.Failure("Ticket not found");
                }
            }
            return Result<Ticket>.Success(ticket);
        }

        public async Task<Result<bool>> ChangeStatusAsync(int id, TicketStatus newStatus)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if(ticket == null)
            {
                return Result<bool>.Failure("Ticket not found");
            }
            var allowed = _ticketWorkflowService.CanChangeStatus(ticket.Status,newStatus);

            if (!allowed)
            {
                return Result<bool>.Failure("Invalid status transition.");
            }
            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.Now;
            var updated = await _ticketRepository.UpdateAsync(ticket);
            if (!updated)
            {
                return Result<bool>.Failure("Unable to update ticket status.");
            }
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> AssignTicketAsync(int id,string agentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return Result<bool>.Failure("Ticket not found");
            }
            var companyId = _currentUserService.GetCompanyId();
            var agents = await _userRepository.GetAgentsByCompanyIdAsync(companyId);
            var agent = agents.FirstOrDefault(a => a.Id == agentId);
            if (agent == null)
            {
                return Result<bool>.Failure("Invalid agent");
            }
            ticket.AssignedToUserId = agentId;
            if (ticket.Status == TicketStatus.New)
            {
                ticket.Status = TicketStatus.Assigned;
            }
            ticket.UpdatedAt = DateTime.Now;
            var updated = await _ticketRepository.UpdateAsync(ticket);
            if (!updated)
            {
                return Result<bool>.Failure("Unable to assign ticket.");
            }
            return Result<bool>.Success(true);
        }
    }
}