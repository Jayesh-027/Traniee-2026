using HelpdeskSystem.Helpers;
using HelpdeskSystem.Models;
using HelpdeskSystem.Repositories;
using HelpdeskSystem.ViewModels;

namespace HelpdeskSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ICurrentUserService _currentUserService;

        public UserService( IUserRepository userRepository,IAuthRepository authRepository,ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<ApplicationUser>> GetAgentsAsync()
        {
            var companyId = _currentUserService.GetCompanyId();

            return await _userRepository.GetAgentsByCompanyIdAsync(companyId);
        }

        public async Task<Result<bool>> CreateAgentAsync(CreateAgentViewModel model)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return Result<bool>.Failure( "User with this email already exists");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                CompanyId = _currentUserService.GetCompanyId()
            };

            var createResult = await _authRepository.CreateUserAsync(
                user,
                model.Password);

            if (!createResult.Succeeded)
            {
                return Result<bool>.Failure("Unable to create agent");
            }

            var roleResult = await _authRepository.AddToRoleAsync(user, "Agent");

            if (!roleResult.Succeeded)
            {
                return Result<bool>.Failure("Unable to assign Agent role");
            }

            return Result<bool>.Success(true);
        }
    }
}