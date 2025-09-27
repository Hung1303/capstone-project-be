using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<BillingRecord> BillingRecords { get; set; }
        public DbSet<CenterProfile> CenterProfiles { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<GeneratedReport> GeneratedReports { get; set; }
        public DbSet<ParentProfile> ParentProfiles { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<SuspensionRecord> SuspensionRecords { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<User> Users { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(x => x.Email).IsUnique();
                b.Property(x => x.Email).HasMaxLength(256).IsRequired();
                b.Property(x => x.FullName).HasMaxLength(256).IsRequired();
            });

            modelBuilder.Entity<TeacherProfile>(b =>
            {
                b.HasIndex(x => x.UserId).IsUnique();
            });
            modelBuilder.Entity<CenterProfile>(b =>
            {
                b.HasIndex(x => x.UserId).IsUnique();
                b.Property(x => x.Name).HasMaxLength(256).IsRequired();
                b.Property(x => x.LicenseNumber).HasMaxLength(128).IsRequired();
            });
            modelBuilder.Entity<StudentProfile>(b =>
            {
                b.HasIndex(x => x.UserId).IsUnique();
                b.Property(x => x.GradeLevel).HasMaxLength(32);
            });
            modelBuilder.Entity<ParentProfile>(b =>
            {
                b.HasIndex(x => x.UserId).IsUnique();
            });

            modelBuilder.Entity<Course>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(256).IsRequired();
                b.Property(x => x.Subject).HasMaxLength(128).IsRequired();
                b.Property(x => x.Location).HasMaxLength(512).IsRequired();
                b.Property(x => x.TuitionFee).HasPrecision(18, 2);
                b.HasMany(x => x.Schedules).WithOne().HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Enrollments).WithOne().HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Reviews).WithOne().HasForeignKey(r => r.CourseId).OnDelete(DeleteBehavior.Cascade);
                b.ToTable(t => t.HasCheckConstraint(
                    "CK_Course_Owner",
                    "(TeacherProfileId IS NOT NULL AND CenterProfileId IS NULL) OR (TeacherProfileId IS NULL AND CenterProfileId IS NOT NULL)"
                ));
            });

            modelBuilder.Entity<ClassSchedule>(b =>
            {
                b.Property(x => x.RoomOrLink).HasMaxLength(512);
            });

            modelBuilder.Entity<Enrollment>(b =>
            {
                b.HasIndex(x => new { x.CourseId, x.StudentProfileId }).IsUnique();
            });

            modelBuilder.Entity<Review>(b =>
            {
                b.Property(x => x.Comment).HasMaxLength(4000);
                b.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "Rating BETWEEN 1 AND 5"));
            });

            modelBuilder.Entity<ApprovalRequest>(b =>
            {
                b.HasIndex(x => x.CourseId);
            });

            modelBuilder.Entity<SuspensionRecord>(b =>
            {
                b.ToTable(t => t.HasCheckConstraint("CK_Suspension_Target", "(UserId IS NOT NULL) OR (CourseId IS NOT NULL)"));
            });

            modelBuilder.Entity<BillingRecord>(b =>
            {
                b.Property(x => x.Amount).HasPrecision(18, 2);
                b.Property(x => x.Currency).HasMaxLength(8);
                b.HasIndex(x => new { x.BillingType, x.CourseId, x.EnrollmentId });
            });

            modelBuilder.Entity<AuditLog>(b =>
            {
                b.Property(x => x.EntityName).HasMaxLength(256);
                b.Property(x => x.IpAddress).HasMaxLength(64);
            });

            modelBuilder.Entity<GeneratedReport>(b =>
            {
                b.Property(x => x.Format).HasMaxLength(16);
                b.Property(x => x.StoragePath).HasMaxLength(1024);
            });
        }
    }
}
