using System.ComponentModel.DataAnnotations;
using HelpdeskSystem.Models;
namespace HelpdeskSystem.ViewModels
{
    public class CreateTicketViewModel
    {
        [Required(ErrorMessage ="Title is required")]
        [MaxLength(20, ErrorMessage = "Length must be in 20 words")]

        public string Title {  get; set; }

        [Required(ErrorMessage="Description is required")]
        [MaxLength(50,ErrorMessage ="Length must be in 50 words")]
        public string Description {  get; set; }

        [Required]
        public TicketPriority Priority {  get; set; }

    }
}
