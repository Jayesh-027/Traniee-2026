using HelpdeskSystem.Models;
namespace HelpdeskSystem.Repositories
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllAsync();
    }
}
