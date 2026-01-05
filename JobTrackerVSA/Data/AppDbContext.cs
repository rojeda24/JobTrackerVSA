using JobTrackerVSA.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Data
{
    public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.CompanyName).HasMaxLength(200);
            base.OnModelCreating(modelBuilder);
        }
    }
}
