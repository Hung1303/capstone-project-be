using Core.Base;

namespace BusinessObjects
{
    public class TeacherProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public int YearOfExperience { get; set; } = 0;
        public string Qualifications { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Subjects { get; set; } = string.Empty;
        public string? Bio { get; set; }

        public Guid? CenterProfileId { get; set; }

        // Circular 29 verification fields
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public DateTime? VerificationRequestedAt { get; set; }
        public DateTime? VerificationCompletedAt { get; set; }
        public string? VerificationNotes { get; set; }

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual ICollection<TeacherFeedback> TeacherFeedbacks { get; set; } = new List<TeacherFeedback>();
    }
}


