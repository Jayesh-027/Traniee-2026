using System.ComponentModel.DataAnnotations;
using HelpdeskSystem.Models;
namespace HelpdeskSystem.ViewModels
{
    public class CreateTicketViewModel
    {
        public string Title {  get; set; }
        public string Description {  get; set; }
        public TicketPriority Priority {  get; set; }

    }
}
