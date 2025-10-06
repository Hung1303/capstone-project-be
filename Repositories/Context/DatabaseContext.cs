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
        public DbSet<CourseFeedback> CourseFeedbacks { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<SuspensionRecord> SuspensionRecords { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<LessonPlan> LessonPlans { get; set; }
        public DbSet<TeacherFeedback> TeacherFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(x => x.Email).IsUnique();
                b.HasIndex(x => x.UserName).IsUnique();
                b.Property(x => x.UserName).HasMaxLength(256).IsRequired();
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
                b.Property(x => x.CenterName).HasMaxLength(256).IsRequired();
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
                b.HasMany(x => x.CourseFeedbacks).WithOne().HasForeignKey(r => r.CourseId).OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<CourseFeedback>(b =>
            {
                b.Property(x => x.Comment).HasMaxLength(4000);
                b.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "Rating BETWEEN 1 AND 5"));
            });

            modelBuilder.Entity<ApprovalRequest>(b =>
            {
                // Ensure Course relationship
                b.HasOne(a => a.Course)
                    .WithMany() // or .WithOne(c => c.ApprovalRequest) if one-to-one
                    .HasForeignKey(a => a.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Ensure Decider relationship
                b.HasOne(a => a.DecidedByUser)
                    .WithMany()
                    .HasForeignKey(a => a.DecidedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Index
                b.HasIndex(x => x.CourseId);

                // Optional: limit Notes length
                b.Property(x => x.Notes).HasMaxLength(2000);
            });

            modelBuilder.Entity<SuspensionRecord>(b =>
            {
                b.ToTable(t => t.HasCheckConstraint(
                    "CK_Suspension_Target",
                    "(UserId IS NOT NULL) OR (CourseId IS NOT NULL)"
                ));

                // Suspension targeting a User
                b.HasOne(sr => sr.User)
                    .WithMany()
                    .HasForeignKey(sr => sr.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Suspension targeting a Course
                b.HasOne(sr => sr.Course)
                    .WithMany()
                    .HasForeignKey(sr => sr.CourseId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Suspension created by an Admin/Moderator
                b.HasOne(sr => sr.ActionByUser)
                    .WithMany()
                    .HasForeignKey(sr => sr.ActionByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Optional constraints or indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.CourseId);
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

                // Relationship: User → AuditLogs
                b.HasOne(a => a.User)
                    .WithMany(u => u.AuditLogs)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Optional constraints or indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.EntityName);
            });

            modelBuilder.Entity<GeneratedReport>(b =>
            {
                b.Property(x => x.Format).HasMaxLength(16);
                b.Property(x => x.StoragePath).HasMaxLength(1024);
                b.Property(x => x.ParametersJson).HasColumnType("nvarchar(max)");
                b.Property(x => x.FileSizeBytes).HasDefaultValue(0);

                // Relationship: Report requested by a user
                b.HasOne(gr => gr.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(gr => gr.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(x => x.RequestedByUserId);
                b.HasIndex(x => x.ReportType);
            });

            modelBuilder.Entity<Syllabus>(b =>{
                b.HasOne(s => s.Course)
                    .WithOne(c => c.Syllabus)
                    .HasForeignKey<Syllabus>(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.Property(s => s.SyllabusName).HasMaxLength(256).IsRequired();
                b.Property(s => s.Description).HasMaxLength(2000);
                b.Property(s => s.GradeLevel).HasMaxLength(64);
                b.Property(s => s.Subject).HasMaxLength(40);
                b.Property(s => s.AssessmentMethod).HasMaxLength(1000);
                b.Property(s => s.CourseMaterial).HasColumnType("nvarchar(max)");
            });

            modelBuilder.Entity<LessonPlan>(b => {
                b.HasOne(s => s.Syllabus)
                    .WithMany(c => c.LessonPlans)
                    .HasForeignKey(lp => lp.SyllabusId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.Property(s => s.Topic).HasMaxLength(256).IsRequired();
                b.Property(s => s.StudentTask).HasMaxLength(2000);
                b.Property(s => s.MaterialsUsed).HasMaxLength(64);
                b.Property(s => s.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<TeacherFeedback>(b =>
            {
                // Target teacher
                b.HasOne(tf => tf.TeacherProfile)
                    .WithMany(tp => tp.TeacherFeedbacks)
                    .HasForeignKey(tf => tf.TeacherProfileId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ Prevent cascade path loop

                // Reviewer: Student
                b.HasOne(tf => tf.StudentProfile)
                    .WithMany()
                    .HasForeignKey(tf => tf.StudentProfileId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ Changed from SetNull to Restrict

                // Reviewer: Parent
                b.HasOne(tf => tf.ParentProfile)
                    .WithMany()
                    .HasForeignKey(tf => tf.ParentProfileId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ Changed from SetNull to Restrict

                // Moderator (User)
                b.HasOne(tf => tf.ModeratedByUser)
                    .WithMany()
                    .HasForeignKey(tf => tf.ModeratedByUserId)
                    .OnDelete(DeleteBehavior.Restrict); // ✅ Changed to Restrict

                // Constraints
                b.Property(tf => tf.Comment).HasMaxLength(2000);
                b.ToTable(t => t.HasCheckConstraint("CK_TeacherFeedback_Rating", "Rating BETWEEN 1 AND 5"));
                b.ToTable(t => t.HasCheckConstraint(
                    "CK_TeacherFeedback_Reviewer",
                    "(StudentProfileId IS NOT NULL) OR (ParentProfileId IS NOT NULL)"
                ));

                b.HasIndex(tf => tf.TeacherProfileId);
            });
        }
    }
}
