using System.Security.Claims;

namespace HelpdeskSystem.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCompanyId()
        {
            var companyId = _httpContextAccessor.HttpContext?.User.FindFirst("CompanyId")?.Value;
            if (companyId == null)
            {
                return 0;
            }

            return Convert.ToInt32(companyId);
        }


    }
}