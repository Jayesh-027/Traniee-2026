namespace HelpdeskSystem.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public int TicketId { get; set; }
       public Ticket Ticket { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Comment { get; set; }
       public DateTime CreatedAt { get; set; }
    }
}
