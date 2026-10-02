using HelpdeskSystem.Helpers;
using HelpdeskSystem.Models;
using HelpdeskSystem.Repositories;
using HelpdeskSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


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

        public async Task<Result<bool>> RegisterAsync(RegisterViewModel model)
        {
            var existinguser = await _authRepository.GetUserByEmailAsync(model.Email);
            if(existinguser != null)
            {
                return Result<bool>.Failure("User with this email already exists");
            }
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                CompanyId = model.CompanyId
            };
            var createResult = await _authRepository.CreateUserAsync(user,model.Password);
            if (!createResult.Succeeded)
            {
                return Result<bool>.Failure("Unable to create user");
            }
            var roleResult = await _authRepository.AddToRoleAsync(user,"Customer");
            if (!roleResult.Succeeded)
            {
                return Result<bool>.Failure("Unable to assign Customer role");
            }
            return Result<bool>.Success(true);
        }


        public async Task<Result<bool>> LoginAsync(LoginViewModel model)
        {
            var user = await _authRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return Result<bool>.Failure("Invalid Email or Pass");
            }
           var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);
            if (!result.Succeeded)
            {
                return Result<bool>.Failure("Invalid Email or Pass");
            }
            var claims = new List<Claim>
            {
                new Claim("CompanyId", user.CompanyId.ToString())
            };
            await _signInManager.SignInWithClaimsAsync(user, false, claims);
            return Result<bool>.Success(true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}