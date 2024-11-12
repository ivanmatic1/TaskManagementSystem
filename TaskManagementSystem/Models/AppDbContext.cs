using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithMany(u => u.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectMembers", 
                    j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
                    j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId")
                );

            modelBuilder.Entity<ProjectTask>()
                .HasMany(t => t.AssignedUsers)
                .WithMany(u => u.Tasks)
                .UsingEntity(j => j.ToTable("TaskAssignments"));
        }

    }
}
