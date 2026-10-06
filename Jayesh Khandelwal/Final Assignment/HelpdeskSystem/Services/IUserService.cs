using HelpdeskSystem.Helpers;
using HelpdeskSystem.Models;
using HelpdeskSystem.ViewModels;

namespace HelpdeskSystem.Services
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAgentsAsync();
        Task<Result<bool>> CreateAgentAsync(CreateAgentViewModel model);
    }
}