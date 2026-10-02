using HelpdeskSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpdeskSystem.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
    }
}