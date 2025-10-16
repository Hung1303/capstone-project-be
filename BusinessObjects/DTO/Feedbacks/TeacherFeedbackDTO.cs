using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.Feedbacks
{
    public class CreateTeacherFeedbackRequest
    {
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class TeacherFeedbackResponse
    {
        public Guid Id { get; set; }
        public Guid TeacherProfileId { get; set; }
        public Guid? StudentProfileId { get; set; }
        public Guid? ParentProfileId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset SubmittedAt { get; set; } = DateTimeOffset.UtcNow;
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }

    public class TeacherFeedbackModerationRequest
    {
        public ReviewStatus Status { get; set; }
        public string? ModerationNotes { get; set; }
    }

    public class UpdateTeacherFeedbackRequest
    {
        [Range(1, 5)]
        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }

}
