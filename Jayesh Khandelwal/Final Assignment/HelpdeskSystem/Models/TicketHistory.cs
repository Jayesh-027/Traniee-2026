namespace HelpdeskSystem.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? IP { get; set; }
        public DateTime Timestamp { get; set; }
    }
}