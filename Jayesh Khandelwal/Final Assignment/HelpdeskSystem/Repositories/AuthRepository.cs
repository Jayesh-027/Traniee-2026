using HelpdeskSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpdeskSystem.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user,string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user,string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }
    }
}