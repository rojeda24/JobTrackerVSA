using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Data
{
    public class AppDbContext (DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : DbContext(options)
    {
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.CompanyName).HasMaxLength(200);

            // Global Query Filter: Only show data for current user
            modelBuilder.Entity<JobApplication>()
                .HasQueryFilter(j => j.UserId == currentUserService.UserId);

            modelBuilder.Entity<Interview>()
                .HasQueryFilter(i => i.JobApplication.UserId == currentUserService.UserId);

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(j => j.Interviews)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
