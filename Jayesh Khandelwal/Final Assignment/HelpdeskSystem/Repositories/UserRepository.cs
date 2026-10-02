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
            return agents
                .Where(a => a.CompanyId == companyId)
                .ToList();
        }
    }
}