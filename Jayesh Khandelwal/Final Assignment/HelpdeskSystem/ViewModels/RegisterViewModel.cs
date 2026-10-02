using System.ComponentModel.DataAnnotations;

namespace HelpdeskSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Email is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "confirm your password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Select Company")]
        [Range(1, int.MaxValue)]
        public int CompanyId { get; set; }
    }
}
