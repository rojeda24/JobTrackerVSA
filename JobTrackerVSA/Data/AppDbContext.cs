using JobTrackerVSA.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTrackerVSA.Web.Data
{
    public class AppDbContext (DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<Interview> Interviews => Set<Interview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobApplication>()
                .Property(j => j.CompanyName).HasMaxLength(200);

            modelBuilder.Entity<Interview>()
                .HasOne(i => i.JobApplication)
                .WithMany(j => j.Interviews)
                .HasForeignKey(i => i.JobApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
