using System.ComponentModel.DataAnnotations;

namespace HelpdeskSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public int CompanyId {  get; set; }
        public Company Company { get; set; }
        public string CreatedByUserId { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
        public DateTime FirstResponseDue { get; set; }
        public DateTime ResolutionDue { get; set; }
        public DateTime? FirstRespondedAt { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<TicketComment> Comments { get; set; }
        public ICollection<TicketHistory> History { get; set; }
    }
}
