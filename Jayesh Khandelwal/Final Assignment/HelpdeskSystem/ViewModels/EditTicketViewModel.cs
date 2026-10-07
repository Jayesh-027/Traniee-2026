using System.ComponentModel.DataAnnotations;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.ViewModels
{
    public class EditTicketViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }
        public byte[] RowVersion { get; set; }

    }
}