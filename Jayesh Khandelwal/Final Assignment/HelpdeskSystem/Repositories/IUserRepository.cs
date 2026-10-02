using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAgentsByCompanyIdAsync(int companyId);
    }
}
