using Core.Base;

namespace BusinessObjects
{
    public class Review : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ReviewStatus Status { get; set; } = ReviewStatus.PendingModeration;
        public Guid? ModeratedByUserId { get; set; }
        public DateTimeOffset? ModeratedAt { get; set; }
        public string? ModerationNotes { get; set; }
    }
}


