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
                    "(\"TeacherProfileId\" IS NOT NULL AND \"CenterProfileId\" IS NULL) OR (\"TeacherProfileId\" IS NULL AND \"CenterProfileId\" IS NOT NULL)"
                ));
            });

            modelBuilder.Entity<ClassSchedule>(b =>
            {
                b.Property(x => x.RoomOrLink).HasMaxLength(512);
            });

            modelBuilder.Entity<Enrollment>(b =>
            {
                b.HasIndex(x => new { x.CourseId, x.StudentProfileId }).IsUnique();

                // 🔹 Thiết lập quan hệ 1 Course - nhiều Enrollment
                b.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<CourseFeedback>(b =>
            {
                b.Property(x => x.Comment).HasMaxLength(4000);
                b.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "\"Rating\" BETWEEN 1 AND 5"));
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
                    "(\"UserId\" IS NOT NULL) OR (\"CourseId\" IS NOT NULL)"
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
                b.Property(x => x.ParametersJson).HasColumnType("text");
                b.Property(x => x.FileSizeBytes).HasDefaultValue(0);

                // Relationship: Report requested by a user
                b.HasOne(gr => gr.RequestedByUser)
                    .WithMany()
                    .HasForeignKey(gr => gr.RequestedByUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(x => x.RequestedByUserId);
                b.HasIndex(x => x.ReportType);
            });

            modelBuilder.Entity<Syllabus>(b =>
            {
                b.HasOne(s => s.Course)
                    .WithOne(c => c.Syllabus)
                    .HasForeignKey<Syllabus>(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.Property(s => s.SyllabusName).HasMaxLength(256).IsRequired();
                b.Property(s => s.Description).HasMaxLength(2000);
                b.Property(s => s.GradeLevel).HasMaxLength(64);
                b.Property(s => s.Subject).HasMaxLength(40);
                b.Property(s => s.AssessmentMethod).HasMaxLength(1000);
                b.Property(s => s.CourseMaterial).HasColumnType("text");
            });

            modelBuilder.Entity<LessonPlan>(b =>
            {
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
                b.ToTable(t => t.HasCheckConstraint("CK_TeacherFeedback_Rating", "\"Rating\" BETWEEN 1 AND 5"));
                b.ToTable(t => t.HasCheckConstraint(
                    "CK_TeacherFeedback_Reviewer",
                    "(\"StudentProfileId\" IS NOT NULL) OR (\"ParentProfileId\" IS NOT NULL)"
                ));

                b.HasIndex(tf => tf.TeacherProfileId);
            });

            // Seed data for Users and Profiles
            var now = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Users
            var userAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var userCenterActiveId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var userCenterPendingId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var userTeacherActiveId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var userTeacherPendingId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var userParentActiveId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var userStudentActiveId = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var userStudentPendingId = Guid.Parse("88888888-8888-8888-8888-888888888888");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = userAdminId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "admin@example.com",
                    UserName = "admin",
                    PasswordHash = "d033e22ae348aeb5660fc2140aec35850c4da997", // sha1("admin") placeholder
                    FullName = "System Admin",
                    PhoneNumber = "+10000000000",
                    Role = Core.Base.UserRole.Admin,
                    Status = Core.Base.AccountStatus.Active
                },
                new User
                {
                    Id = userCenterActiveId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "center.active@example.com",
                    UserName = "center_active",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f", // sha256("Password123!")
                    FullName = "Emily Clark",
                    PhoneNumber = "+10000000001",
                    Role = Core.Base.UserRole.Center,
                    Status = Core.Base.AccountStatus.Active
                },
                new User
                {
                    Id = userCenterPendingId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "center.pending@example.com",
                    UserName = "center_pending",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "Michael Brown",
                    PhoneNumber = "+10000000002",
                    Role = Core.Base.UserRole.Center,
                    Status = Core.Base.AccountStatus.Pending
                },
                new User
                {
                    Id = userTeacherActiveId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "teacher.jane@example.com",
                    UserName = "teacher_jane",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "Jane Doe",
                    PhoneNumber = "+10000000003",
                    Role = Core.Base.UserRole.Teacher,
                    Status = Core.Base.AccountStatus.Active
                },
                new User
                {
                    Id = userTeacherPendingId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "teacher.john@example.com",
                    UserName = "teacher_john",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "John Smith",
                    PhoneNumber = "+10000000004",
                    Role = Core.Base.UserRole.Teacher,
                    Status = Core.Base.AccountStatus.Pending
                },
                new User
                {
                    Id = userParentActiveId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "parent.liam@example.com",
                    UserName = "parent_liam",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "Liam Johnson",
                    PhoneNumber = "+10000000005",
                    Role = Core.Base.UserRole.Parent,
                    Status = Core.Base.AccountStatus.Active
                },
                new User
                {
                    Id = userStudentActiveId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "student.ava@example.com",
                    UserName = "student_ava",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "Ava Johnson",
                    PhoneNumber = "+10000000006",
                    Role = Core.Base.UserRole.Student,
                    Status = Core.Base.AccountStatus.Active
                },
                new User
                {
                    Id = userStudentPendingId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    Email = "student.noah@example.com",
                    UserName = "student_noah",
                    PasswordHash = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f",
                    FullName = "Noah Williams",
                    PhoneNumber = "+10000000007",
                    Role = Core.Base.UserRole.Student,
                    Status = Core.Base.AccountStatus.Pending
                }
            );

            // Center Profiles
            var centerActiveProfileId = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var centerPendingProfileId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            modelBuilder.Entity<CenterProfile>().HasData(
                new CenterProfile
                {
                    Id = centerActiveProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userCenterActiveId,
                    CenterName = "Bright Future Center",
                    OwnerName = "Emily Clark",
                    LicenseNumber = "LIC-2024-0001",
                    IssueDate = new DateOnly(2024, 1, 15),
                    LicenseIssuedBy = "Education Dept",
                    Address = "123 Learning Ave, Cityville",
                    ContactEmail = "contact@brightfuture.example.com",
                    ContactPhone = "+10000001001"
                },
                new CenterProfile
                {
                    Id = centerPendingProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userCenterPendingId,
                    CenterName = "New Horizons Center",
                    OwnerName = "Michael Brown",
                    LicenseNumber = "LIC-2025-0005",
                    IssueDate = new DateOnly(2025, 2, 1),
                    LicenseIssuedBy = "Education Dept",
                    Address = "456 Discovery Rd, Townsburg",
                    ContactEmail = "hello@newhorizons.example.com",
                    ContactPhone = "+10000001002"
                }
            );

            // Teacher Profiles
            var teacherJaneProfileId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var teacherJohnProfileId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            modelBuilder.Entity<TeacherProfile>().HasData(
                new TeacherProfile
                {
                    Id = teacherJaneProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userTeacherActiveId,
                    YearOfExperience = 5,
                    Qualifications = "B.Ed, M.Ed",
                    LicenseNumber = "TCH-2020-123",
                    Subjects = "Math,Physics",
                    Bio = "Experienced STEM teacher",
                    CenterProfileId = centerActiveProfileId
                },
                new TeacherProfile
                {
                    Id = teacherJohnProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userTeacherPendingId,
                    YearOfExperience = 2,
                    Qualifications = "B.Sc",
                    LicenseNumber = "TCH-2023-456",
                    Subjects = "Chemistry",
                    Bio = "Chemistry enthusiast",
                    CenterProfileId = null
                }
            );

            // Parent Profile
            var parentLiamProfileId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
            modelBuilder.Entity<ParentProfile>().HasData(
                new ParentProfile
                {
                    Id = parentLiamProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userParentActiveId,
                    Address = "789 Family St, Suburbia",
                    PhoneSecondary = "+10000002002"
                }
            );

            // Student Profiles
            var studentAvaProfileId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
            var studentNoahProfileId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

            modelBuilder.Entity<StudentProfile>().HasData(
                new StudentProfile
                {
                    Id = studentAvaProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userStudentActiveId,
                    SchoolName = "City High School",
                    GradeLevel = "10",
                    ParentProfileId = parentLiamProfileId
                },
                new StudentProfile
                {
                    Id = studentNoahProfileId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    UserId = userStudentPendingId,
                    SchoolName = "Town Middle School",
                    GradeLevel = "8",
                    ParentProfileId = parentLiamProfileId
                }
            );
        }
    }
}
