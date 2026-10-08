using HelpdeskSystem.Models;

namespace HelpdeskSystem.ViewModels
{
    public class TicketListViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Assignee { get; set; }
        public bool Overdue { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public int Page { get; set; }

        public List<ApplicationUser> Agents { get; set; }
    }
}