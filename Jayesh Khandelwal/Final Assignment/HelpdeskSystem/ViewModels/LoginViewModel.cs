using System.ComponentModel.DataAnnotations;

namespace HelpdeskSystem.ViewModels
{
    public class LoginViewModel

    {
        [Required(ErrorMessage ="Email is required")]
        public string Email {  get; set; }

        [Required(ErrorMessage ="Password is Required")]
        [MinLength(5,ErrorMessage ="PassLength Must be Greater Than 5")]
        public string Password { get; set; }
       
    }
}
