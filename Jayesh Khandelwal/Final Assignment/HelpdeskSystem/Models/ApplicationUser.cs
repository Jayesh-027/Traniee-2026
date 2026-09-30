using Microsoft.AspNetCore.Identity;

namespace HelpdeskSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int CompanyId { get; set; }

        public Company Company { get; set; }
    }
}