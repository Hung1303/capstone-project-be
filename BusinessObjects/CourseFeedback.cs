using Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    public class CourseFeedback : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        [StringLength(4000)]
        public string Comment { get; set; } = string.Empty;
        public ReviewStatus Status { get; set; } = ReviewStatus.PendingModeration;
        public Guid? ModeratedByUserId { get; set; }
        public DateTimeOffset? ModeratedAt { get; set; }
        [StringLength(500)]
        public string? ModerationNotes { get; set; }


        public StudentProfile? StudentProfile { get; set; }
        public ParentProfile? ParentProfile { get; set; }
        public User? ModeratedByUser { get; set; }
    }

}


