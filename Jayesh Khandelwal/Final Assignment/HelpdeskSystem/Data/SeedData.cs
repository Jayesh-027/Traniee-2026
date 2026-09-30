using HelpdeskSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
             RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "CompanyAdmin", "Agent", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            if (!await context.Companies.AnyAsync())
            {
                context.Companies.AddRange(new Company
                {
                    Name = "Company A"
                },
                    new Company
                    {
                        Name = "Company B"
                    });
                await context.SaveChangesAsync();
            }

            var companies = await context.Companies
                .OrderBy(c => c.Id)
                .Take(2)
                .ToListAsync();

            var companyA = companies[0];
            var companyB = companies[1];
            //Company A Data
            var adminA = await CreateUser(
               userManager,
               "adminA@gmail.com",
               "Admin@123",
               companyA.Id,
               "CompanyAdmin");

            var agentA = await CreateUser(
                userManager,
                "agentA@gmail.com",
                "Agent@123",
                companyA.Id,
                "Agent");

            var customerA = await CreateUser(
                userManager,
                "customerA@gmail.com",
                "Customer@123",
                companyA.Id,
                "Customer");

            //Company B DATA
            var adminB = await CreateUser(
                userManager,
                "adminB@gmail.com",
                "Admin@123",
                companyB.Id,
                "CompanyAdmin");

            var agentB = await CreateUser(
                userManager,
                "agentB@gmail.com",
                "Agent@123",
                companyB.Id,
                "Agent");

            var customerB = await CreateUser(
                userManager,
                "customerB@gmail.com",
                "Customer@123",
                companyB.Id,
                "Customer");

            //Tickets
            if (!await context.Tickets.AnyAsync())
            {
                context.Tickets.AddRange(
                    new Ticket
                    {
                        Title = "Login Issue",
                        Description = "I am not able to login",
                        Status = TicketStatus.New,
                        Priority = TicketPriority.High,
                        CompanyId = companyA.Id,
                        Company = companyA,
                        CreatedByUserId = customerA.Id,
                        CreatedByUser = customerA,
                        FirstResponseDue = DateTime.Now.AddHours(1),
                        ResolutionDue = DateTime.Now.AddHours(8),
                        CreatedAt = DateTime.Now
                    },
                    new Ticket
                    {
                        Title = "Server Issue",
                        Description = "I am not getting response from Server",
                        Status = TicketStatus.New,
                        Priority = TicketPriority.Low,
                        CompanyId = companyB.Id,
                        Company = companyB,
                        CreatedByUserId = customerB.Id,
                        CreatedByUser = customerB,
                        FirstResponseDue = DateTime.Now.AddHours(4),
                        ResolutionDue = DateTime.Now.AddHours(24),
                        CreatedAt = DateTime.Now

                    });
                await context.SaveChangesAsync();
            }
        }
        private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager,
            string email,
            string password,
            int companyId,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return user;
            }

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CompanyId = companyId
            };

            var result = await userManager.CreateAsync(user,password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            return user;
        }
    }
}
