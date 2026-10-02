using HelpdeskSystem.Data;
using HelpdeskSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Repositories
{
    
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Company>> GetAllAsync()
        {
            return await _context.Companies.ToListAsync();
        }
    }
}
