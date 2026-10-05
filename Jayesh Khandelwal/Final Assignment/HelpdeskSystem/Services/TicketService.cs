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
        private readonly ITicketCommentRepository _ticketCommentRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;

        public TicketService(ITicketRepository ticketRepository,ICurrentUserService currentUserService,IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ITicketWorkflowService ticketWorkflowService,ITicketCommentRepository ticketCommentRepository, ITicketHistoryRepository ticketHistoryRepository)
        {
            _ticketRepository = ticketRepository;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _ticketWorkflowService = ticketWorkflowService;
            _ticketCommentRepository = ticketCommentRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
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
            var userId = user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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

            var created = await _ticketRepository.CreateAsync(ticket);

            if (!created)
            {
                return false;
            }

            await AddHistoryAsync(ticket.Id,"Created",null,ticket.Title);

            return true;
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
            var oldStatus = ticket.Status.ToString();

            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.Now;
            var updated = await _ticketRepository.UpdateAsync(ticket);
            if (!updated)
            {
                return Result<bool>.Failure("Unable to update ticket status.");
            }

            await AddHistoryAsync(id,"Status Changed",oldStatus,newStatus.ToString());

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
            var oldAgentId = ticket.AssignedToUserId;
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
            await AddHistoryAsync(id,"Assigned",oldAgentId,agentId);
            return Result<bool>.Success(true);
        }
        public async Task<Result<bool>> AddCommentAsync(int ticketId,string comment)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return Result<bool>.Failure("Ticket not found.");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                return Result<bool>.Failure("Comment cannot be empty.");
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                .Value;

            if (userId == null)
            {
                return Result<bool>.Failure("User not found.");
            }

            var companyId = _currentUserService.GetCompanyId();
            var ticketComment = new TicketComment
            {
                TicketId = ticketId,
                CompanyId = companyId,
                UserId = userId,
                Comment = comment,
                CreatedAt = DateTime.Now
            };


            if (!ticket.FirstRespondedAt.HasValue &&
                (user.IsInRole("Agent") || user.IsInRole("CompanyAdmin")))
            {
                ticket.FirstRespondedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;

                var ticketUpdated = await _ticketRepository.UpdateAsync(ticket);

                if (!ticketUpdated)
                {
                    return Result<bool>.Failure(
                        "Unable to update first response time.");
                }
            }

            var result = await _ticketCommentRepository
                .AddAsync(ticketComment);

            if (!result)
            {
                return Result<bool>.Failure("Unable to add comment.");
            }

            return Result<bool>.Success(true);
        }

        public async Task<List<TicketComment>> GetCommentsAsync(int ticketId)
        {
            return await _ticketCommentRepository
                .GetByTicketIdAsync(ticketId);
        }
        public async Task<List<TicketHistory>> GetHistoryAsync(int ticketId)
        {
            return await _ticketHistoryRepository
                .GetByTicketIdAsync(ticketId);
        }
        private async Task AddHistoryAsync(int ticketId, string action,string? oldValue,string? newValue)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userId = user?
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?
                .Value;

            if (userId == null)
                return;

            var companyId = _currentUserService.GetCompanyId();

            var ipAddress =
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var history = new TicketHistory
            {
                TicketId = ticketId,
                CompanyId = companyId,
                UserId = userId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                IP = ipAddress,
                Timestamp = DateTime.Now
            };

            await _ticketHistoryRepository.AddAsync(history);
        }

    }

}