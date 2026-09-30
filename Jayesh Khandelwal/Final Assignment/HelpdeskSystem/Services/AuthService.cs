using System.Security.Claims;
using HelpdeskSystem.Models;
using HelpdeskSystem.Repositories;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HelpdeskSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IAuthRepository authRepository,SignInManager<ApplicationUser> signInManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
        }

        public async Task<bool> LoginAsync(LoginViewModel model)
        {
            var user = await _authRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return false;
            }
           var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);
            if (!result.Succeeded)
            {
                return false;
            }
            var claims = new List<Claim>
            {
                new Claim("CompanyId", user.CompanyId.ToString())
            };
            await _signInManager.SignInWithClaimsAsync(user,false,claims);
            return true;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}