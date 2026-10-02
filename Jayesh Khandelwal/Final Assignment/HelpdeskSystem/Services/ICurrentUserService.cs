using System.Security.Claims;

namespace HelpdeskSystem.Services
{
    public interface ICurrentUserService
    {
        int GetCompanyId();
    }
}