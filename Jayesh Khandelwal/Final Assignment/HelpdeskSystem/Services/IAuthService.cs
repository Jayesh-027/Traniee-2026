
using HelpdeskSystem.ViewModels;
namespace HelpdeskSystem.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}