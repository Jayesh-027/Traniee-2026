using HelpdeskSystem.Models;
using HelpdeskSystem.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpdeskSystem.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketHistory> TicketHistories { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.CompanyId)
                    .IsRequired();


                entity.HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            builder.Entity<Ticket>(entity =>
            {
                entity.Property(t => t.CompanyId)
                    .IsRequired();

                entity.Property(t => t.CreatedByUserId)
                    .IsRequired();

                entity.HasOne(t => t.Company)
                    .WithMany()
                    .HasForeignKey(t => t.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(t => t.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedToUser)
                    .WithMany()
                    .HasForeignKey(t => t.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(t => t.RowVersion)
                    .IsRowVersion();
            });

            builder.Entity<TicketComment>(entity =>
            {
                entity.HasOne(c => c.Ticket)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Company)
                    .WithMany()
                    .HasForeignKey(c => c.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<TicketHistory>(entity =>
            {
                entity.HasOne(h => h.Ticket)
                    .WithMany(t => t.History)
                    .HasForeignKey(h => h.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.User)
                    .WithMany()
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Company)
                    .WithMany()
                    .HasForeignKey(h => h.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany()
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Company)
                    .WithMany()
                    .HasForeignKey(n => n.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(n => n.Ticket)
                    .WithMany()
                    .HasForeignKey(n => n.TicketId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            builder.Entity<Ticket>()
                .HasQueryFilter(t => t.CompanyId == _currentUserService.GetCompanyId());

            builder.Entity<TicketComment>()
                .HasQueryFilter(c => c.CompanyId == _currentUserService.GetCompanyId());

            builder.Entity<TicketHistory>()
                .HasQueryFilter(h => h.CompanyId == _currentUserService.GetCompanyId());

            builder.Entity<Notification>()
                .HasQueryFilter(n => n.CompanyId == _currentUserService.GetCompanyId());
        }
    }
}
