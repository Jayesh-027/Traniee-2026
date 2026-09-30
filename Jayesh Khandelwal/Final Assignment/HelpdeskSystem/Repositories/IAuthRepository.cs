using HelpdeskSystem.Models;

namespace HelpdeskSystem.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
    }
}