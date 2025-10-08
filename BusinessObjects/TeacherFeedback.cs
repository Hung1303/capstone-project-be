using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class TeacherFeedback : BaseEntity
    {
        public Guid TeacherProfileId { get; set; }
        public virtual TeacherProfile TeacherProfile { get; set; } = null!;

        public Guid? StudentProfileId { get; set; }
        public virtual StudentProfile? StudentProfile { get; set; }

        public Guid? ParentProfileId { get; set; }
        public virtual ParentProfile? ParentProfile { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;

        public ReviewStatus Status { get; set; } = ReviewStatus.PendingModeration;
        public Guid? ModeratedByUserId { get; set; }
        public virtual User? ModeratedByUser { get; set; }
        public DateTimeOffset? ModeratedAt { get; set; }
        public string? ModerationNotes { get; set; }
    }
}
