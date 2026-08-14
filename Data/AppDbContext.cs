using Microsoft.EntityFrameworkCore;
using SPM.Models;
using StudentProManagement.Models;

namespace SPM.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<TaskStatus_SPM> TaskStatuses { get; set; }

        public DbSet<TaskPriority> TaskPriorities { get; set; }

        public DbSet<ProjectMaster> ProjectMasters { get; set; }

        public DbSet<ProjectAllocation> ProjectAllocations { get; set; }

        public DbSet<SPM_Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<User>()

                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserType)
                .WithMany(ut => ut.Users)
                .HasForeignKey(u => u.UserTypeID)
                .OnDelete(DeleteBehavior.Restrict);

          
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID)
                .OnDelete(DeleteBehavior.Cascade);

           
            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(p => p.Project)
                .WithMany(pm => pm.ProjectAllocations)
                .HasForeignKey(p => p.ProjectID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectAllocation>()
                .HasOne(p => p.Faculty)
                .WithMany()
                .HasForeignKey(p => p.FacultyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProjectAllocation>()
                .Property(p => p.ProgressPercentage)
                .HasPrecision(18, 2);

         
            modelBuilder.Entity<SPM_Task>()
                .HasOne(t => t.ProjectAllocation)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectAllocationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SPM_Task>()
                .HasOne(t => t.TaskStatus)
                .WithMany(s => s.Tasks)
                .HasForeignKey(t => t.TaskStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SPM_Task>()
                .HasOne(t => t.TaskPriority)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.TaskPriorityID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SPM_Task>()
                .Property(t => t.AssignedScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SPM_Task>()
                .Property(t => t.EarnedScore)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SPM_Task>()
                .Property(t => t.ProgressPercentage)
                .HasPrecision(18, 2);
        }
    }

   
}