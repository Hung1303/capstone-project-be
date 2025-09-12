using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        //public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ClassApproval> ClassApprovals { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        //public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<Parent> Parents { get; set; }
       //public DbSet<Payment> Payments { get; set; }
        //public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherApproval> TeacherApprovals { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        //public DbSet<TuitionFee> TuitionFees { get; set; }
        public DbSet<TutoringClass> TutoringClasses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent multiple cascade paths by using Restrict/NoAction delete behaviors
            modelBuilder.Entity<TeacherApproval>()
                .HasOne(x => x.Teacher)
                .WithMany(t => t.TeacherApprovals)
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherApproval>()
                .HasOne(x => x.Admin)
                .WithMany(a => a.TeacherApprovals)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherSubject>()
                .HasOne(x => x.Teacher)
                .WithMany(t => t.TeacherSubjects)
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TutoringClass>()
                .HasOne(x => x.Teacher)
                .WithMany(t => t.TutoringClasses)
                .HasForeignKey(x => x.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassApproval>()
                .HasOne(x => x.TutoringClass)
                .WithMany(c => c.ClassApprovals)
                .HasForeignKey(x => x.TutoringClassId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassApproval>()
                .HasOne(x => x.Admin)
                .WithMany(a => a.ClassApprovals)
                .HasForeignKey(x => x.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent multiple cascade paths between TutoringClass -> Schedule -> ClassSession and direct FK
            modelBuilder.Entity<ClassSession>()
                .HasOne(x => x.TutoringClass)
                .WithMany()
                .HasForeignKey(x => x.TutoringClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attendance: break cascade paths from Enrollment and ClassSession
            modelBuilder.Entity<Attendance>()
                .HasOne(x => x.Enrollment)
                .WithMany(e => e.Attendances)
                .HasForeignKey(x => x.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(x => x.ClassSession)
                .WithMany(c => c.Attendances)
                .HasForeignKey(x => x.ClassSessionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
