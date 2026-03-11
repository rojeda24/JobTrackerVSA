using JobTrackerVSA.Web.Domain;
using JobTrackerVSA.Web.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Data
{
    public class AppDbContext (DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService) : DbContext(options)
    {
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<JobApplicationAnalytics> JobApplicationAnalytics => Set<JobApplicationAnalytics>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.CompanyName).HasMaxLength(150);
            
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.Position).HasMaxLength(150);
            // Standard max length for URLs
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.JobDescriptionUrl).HasMaxLength(2048);
                
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.Notes).HasMaxLength(5120);

            // Limit UserId length based on OpenID Connect spec for 'sub' claim
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.UserId).HasMaxLength(255);

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
                
            modelBuilder.Entity<Interview>()
                .Property(i => i.Notes).HasMaxLength(5120);

            // Analytical view used by Power BI / reporting clients.
            // [Keyless] on the type is sufficient, so we avoid redundant configuration here.
            modelBuilder.Entity<JobApplicationAnalytics>(eb =>
            {
                eb.ToView("vw_JobApplicationAnalytics");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
