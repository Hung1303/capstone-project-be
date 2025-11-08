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
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<CenterVerificationRequest> CenterVerificationRequests { get; set; }
        public DbSet<TeacherVerificationRequest> TeacherVerificationRequests { get; set; }
        public DbSet<CourseResult> CourseResults { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<CenterSubscription> CenterSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }

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
                b.Property(x => x.GradeLevel).IsRequired();
                b.Property(x => x.ClassName).HasMaxLength(50).IsRequired();
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
                b.Property(x => x.GradeLevel).IsRequired();
                //b.HasMany(x => x.Schedules).WithOne().HasForeignKey(s => s.CourseId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.Enrollments).WithOne().HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.CourseFeedbacks).WithOne().HasForeignKey(r => r.CourseId).OnDelete(DeleteBehavior.Cascade);
                // Removed CK_Course_Owner check constraint to allow courses to be owned by center and taught by teacher simultaneously
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

                // 🔹 Thiết lập quan hệ 1 StudentProfile - nhiều Enrollment
                b.HasOne(e => e.StudentProfile)
                    .WithMany(sp => sp.Enrollments)
                    .HasForeignKey(e => e.StudentProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

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

            // TeacherVerificationRequest configuration
            modelBuilder.Entity<TeacherVerificationRequest>(b =>
            {
                b.HasOne(x => x.TeacherProfile)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Inspector)
                    .WithMany()
                    .HasForeignKey(x => x.InspectorId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Admin)
                    .WithMany()
                    .HasForeignKey(x => x.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
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
                b.HasOne(s => s.Subject)
                    .WithOne(c => c.Syllabus)
                    .HasForeignKey<Syllabus>(s => s.SubjectId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.Property(s => s.SyllabusName).HasMaxLength(256).IsRequired();
                b.Property(s => s.Description).HasMaxLength(2000);
                b.Property(s => s.GradeLevel).HasMaxLength(64);
                //b.Property(s => s.Subject).HasMaxLength(40);
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337", //string123
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    PasswordHash = "0362795b2ee7235b3b4d28f0698a85366703eacf0ba4085796ffd980d7653337",
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
                    ContactPhone = "+10000001001",
                    Status = Core.Base.CenterStatus.Active,
                    VerificationRequestedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    VerificationCompletedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    VerificationNotes = "Successfully verified and approved"
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
                    ContactPhone = "+10000001002",
                    Status = Core.Base.CenterStatus.Pending
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
                    TeachingAtSchool = "City High School",
                    TeachAtClasses = "10A01",
                    CenterProfileId = centerActiveProfileId,
                    VerificationStatus = Core.Base.VerificationStatus.Completed,
                    VerificationRequestedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    VerificationCompletedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    VerificationNotes = "Verified per Circular 29"
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
                    TeachingAtSchool = "City High School",
                    TeachAtClasses = "08A05",
                    CenterProfileId = centerActiveProfileId,
                    VerificationStatus = Core.Base.VerificationStatus.Pending,
                    VerificationRequestedAt = null,
                    VerificationCompletedAt = null,
                    VerificationNotes = null
                }
            );

            // Seed Teacher Verification Requests (use valid fixed GUIDs)
            var teacherJaneVerificationId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var teacherJohnVerificationId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            modelBuilder.Entity<TeacherVerificationRequest>().HasData(
                new TeacherVerificationRequest
                {
                    Id = teacherJaneVerificationId,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    IsDeleted = false,
                    TeacherProfileId = teacherJaneProfileId,
                    Status = Core.Base.VerificationStatus.Completed,
                    RequestedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CompletedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    Notes = "All documents verified",
                    QualificationCertificatePath = "/docs/teachers/jane/qualification.pdf",
                    EmploymentContractPath = "/docs/teachers/jane/contract.pdf",
                    ApprovalFromCenterPath = "/docs/teachers/jane/center-approval.pdf",
                    OtherDocumentsPath = null,
                    InspectorId = null,
                    AdminId = null
                },
                new TeacherVerificationRequest
                {
                    Id = teacherJohnVerificationId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJohnProfileId,
                    Status = Core.Base.VerificationStatus.Pending,
                    RequestedAt = null,
                    CompletedAt = null,
                    Notes = null,
                    QualificationCertificatePath = null,
                    EmploymentContractPath = null,
                    ApprovalFromCenterPath = null,
                    OtherDocumentsPath = null,
                    InspectorId = null,
                    AdminId = null
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
                    SchoolYear = "2024-2025",
                    GradeLevel = 10,
                    ClassName = "10A01",
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
                    SchoolYear = "2024-2025",
                    GradeLevel = 8,
                    ClassName = "08A12",
                    ParentProfileId = parentLiamProfileId
                }
            );

            // CenterVerificationRequest configuration
            modelBuilder.Entity<CenterVerificationRequest>(b =>
            {
                b.HasOne(x => x.CenterProfile)
                    .WithMany(x => x.VerificationRequests)
                    .HasForeignKey(x => x.CenterProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Inspector)
                    .WithMany()
                    .HasForeignKey(x => x.InspectorId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Admin)
                    .WithMany()
                    .HasForeignKey(x => x.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SubscriptionPackage configuration
            modelBuilder.Entity<SubscriptionPackage>(b =>
            {
                b.Property(x => x.PackageName).HasMaxLength(256).IsRequired();
                b.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
                b.Property(x => x.Description).HasMaxLength(2000);
                b.HasIndex(x => x.Tier);
            });

            // CenterSubscription configuration
            modelBuilder.Entity<CenterSubscription>(b =>
            {
                b.HasOne(x => x.CenterProfile)
                    .WithMany(c => c.Subscriptions)
                    .HasForeignKey(x => x.CenterProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.SubscriptionPackage)
                    .WithMany(p => p.CenterSubscriptions)
                    .HasForeignKey(x => x.SubscriptionPackageId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.CenterProfileId);
                b.HasIndex(x => x.SubscriptionPackageId);
                b.HasIndex(x => new { x.CenterProfileId, x.Status });
            });

            // Seed Subscription Packages
            var basicPackageId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111");
            var standardPackageId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222");
            var premiumPackageId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333");
            var enterprisePackageId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-444444444444");

            modelBuilder.Entity<SubscriptionPackage>().HasData(
                new SubscriptionPackage
                {
                    Id = basicPackageId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    PackageName = "Basic Plan",
                    Tier = Core.Base.SubscriptionPackageTier.Basic,
                    MaxCoursePosts = 5,
                    MonthlyPrice = 500000m, // 500,000 VND
                    Description = "Perfect for small centers just starting out. Post up to 5 courses per month.",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new SubscriptionPackage
                {
                    Id = standardPackageId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    PackageName = "Standard Plan",
                    Tier = Core.Base.SubscriptionPackageTier.Standard,
                    MaxCoursePosts = 15,
                    MonthlyPrice = 1500000m, // 1,500,000 VND
                    Description = "Ideal for growing centers. Post up to 15 courses per month.",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new SubscriptionPackage
                {
                    Id = premiumPackageId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    PackageName = "Premium Plan",
                    Tier = Core.Base.SubscriptionPackageTier.Premium,
                    MaxCoursePosts = 50,
                    MonthlyPrice = 4000000m, // 4,000,000 VND
                    Description = "Best for established centers. Post up to 50 courses per month.",
                    IsActive = true,
                    DisplayOrder = 3
                },
                new SubscriptionPackage
                {
                    Id = enterprisePackageId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    PackageName = "Enterprise Plan",
                    Tier = Core.Base.SubscriptionPackageTier.Enterprise,
                    MaxCoursePosts = 200,
                    MonthlyPrice = 10000000m, // 10,000,000 VND
                    Description = "Unlimited growth potential. Post up to 200 courses per month.",
                    IsActive = true,
                    DisplayOrder = 4
                }
            );

            // Seed Center Subscription for the active center (Bright Future Center)
            var centerSubscriptionId = Guid.Parse("bbbbbbbb-cccc-dddd-eeee-111111111111");
            var subscriptionStartDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var subscriptionEndDate = subscriptionStartDate.AddMonths(1); // One month from start

            modelBuilder.Entity<CenterSubscription>().HasData(
                new CenterSubscription
                {
                    Id = centerSubscriptionId,
                    CreatedAt = subscriptionStartDate,
                    LastUpdatedAt = subscriptionStartDate,
                    IsDeleted = false,
                    CenterProfileId = centerActiveProfileId,
                    SubscriptionPackageId = standardPackageId, // Standard Plan (15 courses/month)
                    Status = Core.Base.SubscriptionStatus.Active,
                    StartDate = subscriptionStartDate,
                    EndDate = subscriptionEndDate,
                    AutoRenewalEnabled = true,
                    AutoRenewalDate = subscriptionEndDate,
                    CancelledAt = null,
                    CancellationReason = null
                }
            );

            // Seed Subjects
            var mathSubjectId = Guid.Parse("11111111-aaaa-bbbb-cccc-111111111111");
            var physicsSubjectId = Guid.Parse("22222222-aaaa-bbbb-cccc-222222222222");
            var chemistrySubjectId = Guid.Parse("33333333-aaaa-bbbb-cccc-333333333333");
            var englishSubjectId = Guid.Parse("44444444-aaaa-bbbb-cccc-444444444444");

            modelBuilder.Entity<Subject>().HasData(
                new Subject
                {
                    Id = mathSubjectId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectName = "Mathematics",
                    Description = "Algebra, Geometry, and Calculus for high school students"
                },
                new Subject
                {
                    Id = physicsSubjectId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectName = "Physics",
                    Description = "Classical mechanics, thermodynamics, and electromagnetism"
                },
                new Subject
                {
                    Id = chemistrySubjectId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectName = "Chemistry",
                    Description = "Organic and inorganic chemistry fundamentals"
                },
                new Subject
                {
                    Id = englishSubjectId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectName = "English",
                    Description = "English language and literature for academic excellence"
                }
            );

            // Seed Syllabuses (linked to subjects and teachers)
            var mathSyllabusId = Guid.Parse("aaaaaaaa-1111-2222-3333-aaaaaaaaaaaa");
            var physicsSyllabusId = Guid.Parse("bbbbbbbb-1111-2222-3333-bbbbbbbbbbbb");
            var chemistrySyllabusId = Guid.Parse("cccccccc-1111-2222-3333-cccccccccccc");

            modelBuilder.Entity<Syllabus>().HasData(
                new Syllabus
                {
                    Id = mathSyllabusId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectId = mathSubjectId,
                    TeacherProfileId = teacherJaneProfileId,
                    SyllabusName = "Advanced Mathematics Grade 10",
                    Description = "Comprehensive mathematics syllabus covering algebra, geometry, and trigonometry for grade 10 students.",
                    GradeLevel = 10,
                    AssessmentMethod = "Weekly quizzes (20%), Mid-term exam (30%), Final exam (50%)",
                    CourseMaterial = "Textbook: Mathematics Grade 10, Calculator, Graph paper"
                },
                new Syllabus
                {
                    Id = physicsSyllabusId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectId = physicsSubjectId,
                    TeacherProfileId = teacherJaneProfileId,
                    SyllabusName = "Physics Fundamentals Grade 10",
                    Description = "Introduction to physics covering mechanics, energy, and waves for grade 10 students.",
                    GradeLevel = 10,
                    AssessmentMethod = "Lab reports (25%), Weekly assignments (15%), Mid-term (30%), Final (30%)",
                    CourseMaterial = "Textbook: Physics Grade 10, Lab equipment, Scientific calculator"
                },
                new Syllabus
                {
                    Id = chemistrySyllabusId,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SubjectId = chemistrySubjectId,
                    TeacherProfileId = teacherJohnProfileId,
                    SyllabusName = "Chemistry Basics Grade 8",
                    Description = "Basic chemistry concepts including periodic table, chemical reactions, and laboratory safety.",
                    GradeLevel = 8,
                    AssessmentMethod = "Lab practicals (30%), Quizzes (20%), Projects (20%), Final exam (30%)",
                    CourseMaterial = "Textbook: Chemistry Basics, Lab manual, Safety equipment"
                }
            );

            // Seed Lesson Plans (linked to syllabuses)
            var lessonPlan1Id = Guid.Parse("11111111-ffff-eeee-dddd-111111111111");
            var lessonPlan2Id = Guid.Parse("22222222-ffff-eeee-dddd-222222222222");
            var lessonPlan3Id = Guid.Parse("33333333-ffff-eeee-dddd-333333333333");
            var lessonPlan4Id = Guid.Parse("44444444-ffff-eeee-dddd-444444444444");
            var lessonPlan5Id = Guid.Parse("55555555-ffff-eeee-dddd-555555555555");
            var lessonPlan6Id = Guid.Parse("66666666-ffff-eeee-dddd-666666666666");

            modelBuilder.Entity<LessonPlan>().HasData(
                new LessonPlan
                {
                    Id = lessonPlan1Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = mathSyllabusId,
                    Topic = "Introduction to Quadratic Equations",
                    StudentTask = "Complete exercises 1-20 from textbook. Submit solutions before next class.",
                    MaterialsUsed = "Whiteboard, Graphing calculator, Textbook",
                    Notes = "Focus on standard form ax² + bx + c = 0"
                },
                new LessonPlan
                {
                    Id = lessonPlan2Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = mathSyllabusId,
                    Topic = "Graphing Quadratic Functions",
                    StudentTask = "Graph 5 quadratic functions and identify vertex, axis of symmetry",
                    MaterialsUsed = "Graph paper, Graphing software, Calculator",
                    Notes = "Practice identifying maximum and minimum points"
                },
                new LessonPlan
                {
                    Id = lessonPlan3Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = physicsSyllabusId,
                    Topic = "Newton's Laws of Motion",
                    StudentTask = "Lab report on motion experiments. Calculate acceleration from data.",
                    MaterialsUsed = "Lab equipment, Stopwatch, Rulers, Masses",
                    Notes = "Hands-on experiment to demonstrate F = ma"
                },
                new LessonPlan
                {
                    Id = lessonPlan4Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = physicsSyllabusId,
                    Topic = "Energy and Work",
                    StudentTask = "Solve energy conservation problems. Complete worksheet.",
                    MaterialsUsed = "Textbook, Calculator, Diagram sheets",
                    Notes = "Emphasize conservation of energy principle"
                },
                new LessonPlan
                {
                    Id = lessonPlan5Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = chemistrySyllabusId,
                    Topic = "Introduction to Periodic Table",
                    StudentTask = "Memorize first 20 elements. Complete periodic table worksheet.",
                    MaterialsUsed = "Periodic table poster, Flashcards, Worksheets",
                    Notes = "Focus on element symbols and atomic numbers"
                },
                new LessonPlan
                {
                    Id = lessonPlan6Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    SyllabusId = chemistrySyllabusId,
                    Topic = "Chemical Bonding Basics",
                    StudentTask = "Draw Lewis structures for 10 molecules. Lab: observe reactions.",
                    MaterialsUsed = "Molecular model kits, Lab chemicals, Safety goggles",
                    Notes = "Emphasize safety procedures in lab"
                }
            );

            // Seed Class Schedules (for different days and times)
            var classSchedule1Id = Guid.Parse("aaaaaaaa-1111-aaaa-bbbb-111111111111");
            var classSchedule2Id = Guid.Parse("bbbbbbbb-2222-aaaa-bbbb-222222222222");
            var classSchedule3Id = Guid.Parse("cccccccc-3333-aaaa-bbbb-333333333333");
            var classSchedule4Id = Guid.Parse("dddddddd-4444-aaaa-bbbb-444444444444");
            var classSchedule5Id = Guid.Parse("eeeeeeee-5555-aaaa-bbbb-555555555555");

            var courseStartDate = new DateOnly(2025, 2, 1);
            var courseEndDate = new DateOnly(2025, 5, 31);

            modelBuilder.Entity<ClassSchedule>().HasData(
                new ClassSchedule
                {
                    Id = classSchedule1Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJaneProfileId,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0), // 9:00 AM
                    EndTime = new TimeOnly(10, 30), // 10:30 AM
                    StartDate = courseStartDate,
                    EndDate = courseEndDate,
                    RoomOrLink = "Room 101, 123 Learning Ave, Cityville"
                },
                new ClassSchedule
                {
                    Id = classSchedule2Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJaneProfileId,
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(14, 0), // 2:00 PM
                    EndTime = new TimeOnly(15, 30), // 3:30 PM
                    StartDate = courseStartDate,
                    EndDate = courseEndDate,
                    RoomOrLink = "Room 101, 123 Learning Ave, Cityville"
                },
                new ClassSchedule
                {
                    Id = classSchedule3Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJaneProfileId,
                    DayOfWeek = DayOfWeek.Friday,
                    StartTime = new TimeOnly(9, 0), // 9:00 AM
                    EndTime = new TimeOnly(10, 30), // 10:30 AM
                    StartDate = courseStartDate,
                    EndDate = courseEndDate,
                    RoomOrLink = "Room 102, 123 Learning Ave, Cityville"
                },
                new ClassSchedule
                {
                    Id = classSchedule4Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJohnProfileId,
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(15, 0), // 3:00 PM
                    EndTime = new TimeOnly(16, 30), // 4:30 PM
                    StartDate = courseStartDate,
                    EndDate = courseEndDate,
                    RoomOrLink = "Lab Room 1, 123 Learning Ave, Cityville"
                },
                new ClassSchedule
                {
                    Id = classSchedule5Id,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    IsDeleted = false,
                    TeacherProfileId = teacherJaneProfileId,
                    DayOfWeek = DayOfWeek.Saturday,
                    StartTime = new TimeOnly(10, 0), // 10:00 AM
                    EndTime = new TimeOnly(11, 30), // 11:30 AM
                    StartDate = courseStartDate,
                    EndDate = courseEndDate,
                    RoomOrLink = "https://zoom.us/j/1234567890" // Online class example
                }
            );
        }
    }
}
