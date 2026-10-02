
using HelpdeskSystem.Helpers;
using HelpdeskSystem.ViewModels;
namespace HelpdeskSystem.Services
{
    public interface IAuthService
    {
        Task<Result<bool>> RegisterAsync(RegisterViewModel model);
        Task<Result<bool>> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}