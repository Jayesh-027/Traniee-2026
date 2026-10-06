using HelpdeskSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpdeskSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<List<ApplicationUser>> GetAgentsByCompanyIdAsync(int companyId)
        {
            var agents = await _userManager.GetUsersInRoleAsync("Agent");
            return agents .Where(a => a.CompanyId == companyId).ToList();
        }
        public async Task<bool> CreateAgentAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return false;
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Agent");
            return roleResult.Succeeded;
        }
    }
}